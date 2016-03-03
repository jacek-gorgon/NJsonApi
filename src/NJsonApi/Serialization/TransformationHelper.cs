using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NJsonApi.Infrastructure;
using NJsonApi.Exceptions;
using NJsonApi.Serialization.Representations;
using NJsonApi.Serialization.Representations.Relationships;
using NJsonApi.Serialization.Representations.Resources;
using System.Reflection;

namespace NJsonApi.Serialization
{
    internal class TransformationHelper
    {
        private const string IdPlaceholder = "{id}";
        private const string ParentIdPlaceholder = "{parentId}";
        private const string RelatedIdPlaceholder = "{relatedId}";
        private const string MetaCountAttribute = "count";
        private const string SelfLinkKey = "self";
        private readonly IConfiguration configuration;
        private readonly ILinkBuilder linkBuilder;

        public TransformationHelper(IConfiguration configuration, ILinkBuilder linkBuilder)
        {
            this.configuration = configuration;
            this.linkBuilder = linkBuilder;
        }

        public IResourceRepresentation ChooseProperResourceRepresentation(object resource, IEnumerable<SingleResource> representationList)
        {
            return resource is IEnumerable ?
                (IResourceRepresentation)new ResourceCollection(representationList) :
                representationList.Single();
        }

        public List<SingleResource> CreateIncludedRepresentations(List<object> primaryResourceList, IResourceMapping resourceMapping, Context context)
        {
            var includedList = new List<SingleResource>();
            var alreadyVisitedObjects = new HashSet<object>(primaryResourceList);

            foreach (var resource in primaryResourceList)
            {
                includedList.AddRange(
                    AppendIncludedRepresentationRecursive(
                        resource, 
                        resourceMapping, 
                        alreadyVisitedObjects, 
                        context));
            }

            if (includedList.Any())
            {
                return includedList;
            }
            return null;
        }

        private List<SingleResource> AppendIncludedRepresentationRecursive(
            object resource, 
            IResourceMapping resourceMapping, 
            HashSet<object> alreadyVisitedObjects, 
            Context context,
            string parentRelationshipPath = "")
        {
            var includedResources = new List<SingleResource>();

            foreach(var relationship in resourceMapping.Relationships)
            {
                if (relationship.InclusionRule == ResourceInclusionRules.ForceOmit)
                {
                    continue;
                }

                var relatedResources = UnifyObjectsToList(relationship.RelatedResource(resource));
                string relationshipPath = BuildRelationshipPath(parentRelationshipPath, relationship);

                if (!context.IncludedResources.Any(x => x.Contains(relationshipPath)))
                {
                    continue;
                }
                
                foreach (var relatedResource in relatedResources)
                {
                    if (alreadyVisitedObjects.Contains(relatedResource))
                    {
                        continue;
                    }

                    alreadyVisitedObjects.Add(relatedResource);
                    includedResources.Add(
                        CreateResourceRepresentation(relatedResource, relationship.ResourceMapping, context));

                    includedResources.AddRange(
                        AppendIncludedRepresentationRecursive(relatedResource, relationship.ResourceMapping, alreadyVisitedObjects, context, relationshipPath));
                }
            }

            return includedResources;
        }
        
        private string BuildRelationshipPath(string parentRelationshipPath, IRelationshipMapping relationship)
        { 
            if (string.IsNullOrEmpty(parentRelationshipPath))
            {
                return relationship.RelatedBaseResourceType;
            }
            else
            {
                return $"{parentRelationshipPath}.{relationship.RelatedBaseResourceType}";
            }
        }

        public List<object> UnifyObjectsToList(object nestedObject)
        {
            var list = new List<object>();
            if (nestedObject != null)
            {
                if (nestedObject is IEnumerable<object>)
                    list.AddRange((IEnumerable<object>)nestedObject);
                else
                    list.Add(nestedObject);
            }

            return list;
        }

        public void VerifyTypeSupport(Type innerObjectType)
        {
            if (typeof(IMetaDataWrapper).IsAssignableFrom(innerObjectType))
            {
                throw new NotSupportedException(string.Format("Error while serializing type {0}. IEnumerable<MetaDataWrapper<>> is not supported.", innerObjectType));
            }

            if (typeof(IEnumerable).IsAssignableFrom(innerObjectType) && !innerObjectType.GetTypeInfo().IsGenericType)
            {
                throw new NotSupportedException(string.Format("Error while serializing type {0}. Non generic IEnumerable are not supported.", innerObjectType));
            }
        }

        public object UnwrapResourceObject(object objectGraph)
        {
            if (!(objectGraph is IMetaDataWrapper))
            {
                return objectGraph;
            }
            var metadataWrapper = objectGraph as IMetaDataWrapper;
            return metadataWrapper.GetValue();
        }

        public Dictionary<string, object> GetMetadata(object objectGraph)
        {
            if (objectGraph is IMetaDataWrapper)
            {
                var metadataWrapper = objectGraph as IMetaDataWrapper;
                return metadataWrapper.MetaData;
            }
            return null;
        }

        public SingleResource CreateResourceRepresentation(
            object objectGraph, 
            IResourceMapping resourceMapping, 
            Context context)
        {
            var urlBuilder = new UrlBuilder();
            var result = new SingleResource();

            result.Id = resourceMapping.IdGetter(objectGraph).ToString();
            result.Type = resourceMapping.ResourceType;
            result.Attributes = resourceMapping.PropertyGetters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value(objectGraph));
            result.Links = new Dictionary<string, ILink>() { { "self", linkBuilder.FindLink(context, result.Id, resourceMapping) } };

            if (resourceMapping.Relationships.Any())
            {
                result.Relationships = CreateRelationships(objectGraph, result.Id, resourceMapping, context);
            }

            return result;
        }

        private ILink GetUrlFromTemplate(Context context, string urlTemplate, string parentId, string relatedId = null)
        {
            var builder = new UrlBuilder();
            return new SimpleLink
            {
                Href = builder.GetFullyQualifiedUrl(context, urlTemplate.Replace(ParentIdPlaceholder, parentId).Replace(RelatedIdPlaceholder, relatedId))
            };
        }

        public Dictionary<string, IRelationship> CreateRelationships(object objectGraph, string parentId, IResourceMapping resourceMapping, Context context)
        {
            var relationships = new Dictionary<string, IRelationship>();
            foreach (var linkMapping in resourceMapping.Relationships)
            {
                var relationshipName = linkMapping.RelationshipName;
                var rel = new Relationship();
                var relLinks = new RelationshipLinks();

                // Generating "self" link
                if (linkMapping.SelfUrlTemplate != null)
                    relLinks.Self = GetUrlFromTemplate(context, linkMapping.SelfUrlTemplate, parentId);

                if (!linkMapping.IsCollection)
                {
                    string relatedId = null;
                    object relatedInstance = null;
                    if (linkMapping.RelatedResource != null)
                    {
                        relatedInstance = linkMapping.RelatedResource(objectGraph);
                        if (relatedInstance != null)
                            relatedId = linkMapping.ResourceMapping.IdGetter(relatedInstance).ToString();
                    }
                    if (linkMapping.RelatedResourceId != null && relatedId == null)
                    {
                        var id = linkMapping.RelatedResourceId(objectGraph);
                        if (id != null)
                            relatedId = id.ToString();
                    }

                    // Generating "related" link for to-one relationships
                    if (linkMapping.RelatedUrlTemplate != null && relatedId != null)
                        relLinks.Related = GetUrlFromTemplate(context, linkMapping.RelatedUrlTemplate, parentId, relatedId.ToString());


                    if (linkMapping.InclusionRule != ResourceInclusionRules.ForceOmit)
                    {
                        // Generating resource linkage for to-one relationships
                        if (relatedInstance != null)
                            rel.Data = new SingleResourceIdentifier
                            {
                                Id = relatedId,
                                Type = configuration.GetMapping(relatedInstance.GetType()).ResourceType // This allows polymorphic (subtyped) resources to be fully represented
                            };
                        else if (relatedId == null || linkMapping.InclusionRule == ResourceInclusionRules.ForceInclude)
                            rel.Data = new NullResourceIdentifier(); // two-state null case, see NullResourceIdentifier summary
                    }
                }
                else
                {
                    // Generating "related" link for to-many relationships
                    if (linkMapping.RelatedUrlTemplate != null)
                        relLinks.Related = GetUrlFromTemplate(context, linkMapping.RelatedUrlTemplate, parentId);

                    IEnumerable relatedInstance = null;
                    if (linkMapping.RelatedResource != null)
                        relatedInstance = (IEnumerable)linkMapping.RelatedResource(objectGraph);

                    // Generating resource linkage for to-many relationships
                    if (linkMapping.InclusionRule == ResourceInclusionRules.ForceInclude && relatedInstance == null)
                        rel.Data = new MultipleResourceIdentifiers();
                    if (linkMapping.InclusionRule != ResourceInclusionRules.ForceOmit && relatedInstance != null)
                    {
                        var idGetter = linkMapping.ResourceMapping.IdGetter;
                        var identifiers = relatedInstance
                            .Cast<object>()
                            .Select(o => new SingleResourceIdentifier
                            {
                                Id = idGetter(o).ToString(),
                                Type = configuration.GetMapping(o.GetType()).ResourceType // This allows polymorphic (subtyped) resources to be fully represented
                            });
                        rel.Data = new MultipleResourceIdentifiers(identifiers);
                    }

                    // If data is present, count meta attribute is added for convenience
                    if (rel.Data != null)
                        rel.Meta = new Dictionary<string, string> { { MetaCountAttribute, ((MultipleResourceIdentifiers)rel.Data).Count.ToString() } };
                }

                if (relLinks.Self != null || relLinks.Related != null)
                    rel.Links = relLinks;

                if (rel.Data != null || rel.Links != null)
                    relationships.Add(relationshipName, rel);
            }
            return relationships.Any() ? relationships : null;
        }

        public void AssureAllMappingsRegistered(Type type, IConfiguration config)
        {
            if (!config.IsMappingRegistered(type))
            {
                throw new MissingMappingException(type);
            }
        }

        public Dictionary<string, ILink> GetTopLevelLinks(Uri requestUri)
        {
            var topLevel = new Dictionary<string, ILink>();
            topLevel.Add("self", new SimpleLink(requestUri));
            return topLevel;
        }
    }
}