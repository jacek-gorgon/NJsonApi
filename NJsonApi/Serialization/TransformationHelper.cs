using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using NJsonApi.Common.Infrastructure;
using NJsonApi.Exceptions;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations;
using NJsonApi.Serialization.Representations.Relationships;

namespace NJsonApi.Serialization
{
    public class TransformationHelper
    {
        public const int RecursionDepthLimit = 10;
        private const string IdPlaceholder = "{id}";
        private const string ParentIdPlaceholder = "{parentId}";
        private const string RelatedIdPlaceholder = "{relatedId}";
        private const string MetaCountAttribute = "count";
        private const string SelfLinkKey = "self";

        public CompoundDocument HandleException(Exception exception)
        {
            var scfException = exception is NJsonApiBaseException
                ? exception as NJsonApiBaseException
                : new NJsonApiBaseException(exception.Message, exception);

            var compoundDocument = new CompoundDocument
            {
                Errors = new Dictionary<string, Error>
                {
                    { "error", new Error
                    {
                        Id = scfException.Id.ToString(),
                        Title = scfException.Message,
                        Status = ((int)scfException.GetHttpStatusCode()).ToString(CultureInfo.InvariantCulture),
                    }}
                }
            };

            return compoundDocument;
        }

        public CompoundDocument HandleHttpError(HttpError error)
        {
            return new CompoundDocument
            {
                Errors = new Dictionary<string, Error>
                {
                    { "Internal server error", new Error
                    {
                        Status = error.Message,
                    }}
                }
            };
        }

        public IResourceRepresentation GetPrimaryResource(object resource, Configuration config, IResourceMapping resourceMapping, string routePrefix)
        {
            var resourcesList = UnifyObjectsToList(resource);

            var representationList = resourcesList.Select(o => CreateResourceRepresentation(o, config, resourceMapping, routePrefix));

            return resource is IEnumerable ?
                (IResourceRepresentation)new ResourceCollection(representationList) :
                representationList.Single();
        }

        public Dictionary<string, List<object>> CreateLinkedRepresentation(object resource, IResourceMapping resourceMapping)
        {
            var linkedDictionary = new Dictionary<string, List<object>>();
            var depth = 0;
            CreateLinkedRepresentationInner(resource, resourceMapping, linkedDictionary, ref depth, new HashSet<object>());

            var distinctValues = GetDistinctValues(linkedDictionary);

            var objects = (resource as IList);
            if (objects != null)
            {
                List<string> primaryIds = objects
                    .Cast<object>()
                    .Select(r => resourceMapping.IdGetter(r).ToString())
                    .ToList();

                return RemovePrimaryResource(distinctValues, primaryIds, resourceMapping.ResourceType);
            }


            return distinctValues;
        }

        public Dictionary<string, List<object>> RemovePrimaryResource(Dictionary<string, List<object>> linkedDictionary, List<string> primaryResourceId, string resourceType)
        {
            if (!linkedDictionary.ContainsKey(resourceType))
            {
                return linkedDictionary;
            }

            foreach (var dictItem in linkedDictionary.Where(d => d.Key == resourceType))
            {
                IEnumerable<Dictionary<string, object>> linked = dictItem.Value
                    .Cast<Dictionary<string, object>>()
                    .Where(i => i.ContainsKey("id"))
                    .ToList();

                foreach (var linkedInner in linked)
                {
                    if (primaryResourceId.Contains(linkedInner["id"]))
                    {
                        linkedDictionary[dictItem.Key].Remove(linkedInner);
                    }
                }

            }

            return linkedDictionary;
        }

        public void CreateLinkedRepresentationInner(object resource, IResourceMapping resourceMapping, Dictionary<string, List<object>> linkedDictionary, ref int depth, HashSet<object> hashSet)
        {
            if (resource is IEnumerable)
            {
                if (((IEnumerable)resource).OfType<object>().All(hashSet.Contains))
                {
                    return;
                }
                foreach (var res in (IEnumerable)resource)
                {
                    hashSet.Add(res);
                }
            }
            else
            {
                if (hashSet.Contains(resource))
                {
                    return;
                }
                hashSet.Add(resource);
            }

            depth++;

            foreach (var linkMapping in resourceMapping.Relationships.Where(l => l.InclusionRule != ResourceInclusionRules.ForceOmit))
            {
                var key = linkMapping.ResourceMapping.ResourceType;
                var nestedObject = GetNestedResource(resource, linkMapping);
                List<object> nestedObjects = UnifyObjectsToList(nestedObject);

                List<object> linkedObjects = nestedObjects
                    .Select(o => (object)CreateLinkedResourceRepresentation(o, linkMapping.ResourceMapping))
                    .ToList();

                MergeResultToDictionary(linkedDictionary, key, linkedObjects);

                if (linkMapping.ResourceMapping.Relationships.Any() && nestedObjects.Any() && depth < RecursionDepthLimit)
                {

                    CreateLinkedRepresentationInner(nestedObject, linkMapping.ResourceMapping, linkedDictionary, ref depth, hashSet);
                }
            }
        }

        public object GetNestedResource(object resource, IRelationshipMapping linkMapping)
        {
            if (resource is IEnumerable<object>)
            {
                var result = new List<object>();
                foreach (var rsx in (resource as IEnumerable<object>))
                {
                    var innerResource = linkMapping.RelatedResource(rsx);
                    if (innerResource != null)
                    {
                        if (innerResource is IEnumerable<object>)
                        {
                            result.AddRange(innerResource as IEnumerable<object>);
                        }
                        else
                        {
                            result.Add(innerResource);
                        }
                    }
                }
                return result;
            }

            return linkMapping.RelatedResource(resource);
        }

        public List<object> UnifyObjectsToList(object nestedObject)
        {
            if (nestedObject == null)
                return new List<object>();

            var list = new List<object>();
            if (nestedObject is IEnumerable<object>)
                list.AddRange((IEnumerable<object>)nestedObject);
            else
                list.Add(nestedObject);

            return list;
        }

        public void MergeResultToDictionary(Dictionary<string, List<object>> targerDictionary, string key, List<object> objects)
        {
            if (targerDictionary.ContainsKey(key))
            {
                targerDictionary[key].AddRange(objects);
            }
            else
            {
                targerDictionary[key] = objects;
            }
        }

        public Dictionary<string, List<object>> GetDistinctValues(Dictionary<string, List<object>> linkedObjectDictionary)
        {
            var resultDictionary = new Dictionary<string, List<object>>();
            foreach (var dictItem in linkedObjectDictionary)
            {
                var distinctObjects = dictItem.Value
                    .Cast<Dictionary<string, object>>()
                    .Where(i => i.ContainsKey("id"))
                    .GroupBy(i => i["id"])
                    .Select(g => (object)g.First())
                    .ToList();

                resultDictionary[dictItem.Key] = distinctObjects;
            }

            return resultDictionary;
        }

        public Dictionary<string, LinkTemplate> CreateLinkRepresentation(IResourceMapping resourceMapping, string routePrefix)
        {
            var links = new Dictionary<string, LinkTemplate>();

            CreateLinkRepresentationInner(resourceMapping, links, routePrefix);

            return links;
        }

        public void CreateLinkRepresentationInner(IResourceMapping resourceMapping, Dictionary<string, LinkTemplate> links, string routePrefix)
        {
            if (resourceMapping.Relationships.Any())
            {
                var urlBuilder = new UrlBuilder()
                {
                    RoutePrefix = routePrefix
                };

                foreach (var linkMapping in resourceMapping.Relationships)
                {
                    var linkTemplate = new LinkTemplate
                    {
                        Href = urlBuilder.GetFullyQualifiedUrl(linkMapping.ResourceMapping.UrlTemplate),
                        Type = linkMapping.ResourceMapping.ResourceType
                    };

                    var linkKey = string.Format("{0}.{1}", resourceMapping.ResourceType, linkMapping.RelationshipName);
                    if (links.ContainsKey(linkKey)) break;

                    links.Add(linkKey, linkTemplate);

                    if (resourceMapping.Relationships.Any())
                    {
                        CreateLinkRepresentationInner(resourceMapping, links, routePrefix);
                    }
                }
            }
        }

        public void VerifyTypeSupport(Type innerObjectType)
        {
            if (typeof(IMetaDataWrapper).IsAssignableFrom(innerObjectType))
            {
                throw new NotSupportedException(string.Format("Error while serializing type {0}. IEnumerable<MetaDataWrapper<>> is not supported.", innerObjectType));
            }

            if (typeof(IEnumerable).IsAssignableFrom(innerObjectType) && !innerObjectType.IsGenericType)
            {
                throw new NotSupportedException(string.Format("Error while serializing type {0}. Non generic IEnumerable are not supported.", innerObjectType));
            }
        }

        public object GetResourceObject(object objectGraph)
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
            return new Dictionary<string, object>();
        }

        public Type GetObjectType(object objectGraph)
        {
            Type objectType = objectGraph.GetType();
            if (objectGraph is IMetaDataWrapper)
            {
                objectType = objectGraph.GetType().GetGenericArguments()[0];
            }

            if (typeof(IEnumerable).IsAssignableFrom(objectType) && objectType.IsGenericType)
            {
                objectType = objectType.GetGenericArguments()[0];
            }

            return objectType;
        }

        public SingleResource CreateResourceRepresentation(object objectGraph, Configuration config, IResourceMapping resourceMapping, string routePrefix)
        {
            var urlBuilder = new UrlBuilder
            {
                RoutePrefix = routePrefix
            };

            var result = new SingleResource();

            result.Id = resourceMapping.IdGetter(objectGraph).ToString();
            result.Type = resourceMapping.ResourceType;

            result.Attributes = resourceMapping.PropertyGetters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value(objectGraph));

            if (resourceMapping.UrlTemplate != null)
                result.Links[SelfLinkKey] = new SimpleLink { Href = urlBuilder.GetFullyQualifiedUrl(resourceMapping.UrlTemplate.Replace(IdPlaceholder, result.Id)) };

            if (resourceMapping.Relationships.Any())
                result.Relationships = CreateRelationships(objectGraph, config, resourceMapping, routePrefix, result.Id);

            return result;
        }

        private ILink GetUrlFromTemplate(string urlTemplate, string routePrefix, string parentId, string relatedId = null)
        {
            var builder = new UrlBuilder
            {
                RoutePrefix = routePrefix
            };
            return new SimpleLink
            {
                Href = builder.GetFullyQualifiedUrl(urlTemplate.Replace(ParentIdPlaceholder, parentId).Replace(RelatedIdPlaceholder, relatedId))
            };
        }

        public Dictionary<string, IRelationship> CreateRelationships(object objectGraph, Configuration config, IResourceMapping resourceMapping, string routePrefix, string parentId)
        {
            var links = new Dictionary<string, IRelationship>();
            foreach (var linkMapping in resourceMapping.Relationships)
            {
                var relationshipName = linkMapping.RelationshipName;
                var rel = new Relationship();
                var relLinks = new RelationshipLinks();

                // Generating "self" link
                if (linkMapping.SelfUrlTemplate != null)
                    relLinks.Self = GetUrlFromTemplate(linkMapping.SelfUrlTemplate, routePrefix, parentId);

                if (!linkMapping.IsCollection)
                {
                    string relatedId = null;
                    object relatedInstance = null;
                    if (linkMapping.RelatedResource != null)
                    {
                        relatedInstance = linkMapping.RelatedResource(objectGraph);
                        relatedId = linkMapping.ResourceMapping.IdGetter(relatedInstance).ToString();
                    }
                    if (linkMapping.RelatedResourceId != null && relatedId == null)
                        relatedId = linkMapping.RelatedResourceId(objectGraph).ToString();


                    // Generating "related" link for to-one relationships
                    if (linkMapping.RelatedUrlTemplate != null && relatedId != null)
                        relLinks.Related = GetUrlFromTemplate(linkMapping.RelatedUrlTemplate, routePrefix, parentId, relatedId.ToString());


                    if (linkMapping.InclusionRule != ResourceInclusionRules.ForceOmit)
                    {
                        // Generating resource linkage for to-one relationships
                        if (relatedInstance != null)
                            rel.Data = new SingleResourceIdentifier
                            {
                                Id = relatedId,
                                Type = config.GetMapping(relatedInstance.GetType()).ResourceType // This allows polymorphic (subtyped) resources to be fully represented
                            };
                        else if (relatedId == null || linkMapping.InclusionRule == ResourceInclusionRules.ForceInclude)
                            rel.Data = new NullResourceIdentifier(); // two-state null case, see NullResourceIdentifier summary
                    }
                }
                else
                {
                    // Generating "related" link for to-many relationships
                    if (linkMapping.RelatedUrlTemplate != null)
                        relLinks.Related = GetUrlFromTemplate(linkMapping.RelatedUrlTemplate, routePrefix, parentId);

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
                                Type = config.GetMapping(o.GetType()).ResourceType // This allows polymorphic (subtyped) resources to be fully represented
                            });
                        rel.Data = new MultipleResourceIdentifiers(identifiers);
                    }

                    // If data is present, count meta attribute is added for convenience
                    if (rel.Data != null)
                        rel.Meta = new Dictionary<string, string> { { MetaCountAttribute, ((MultipleResourceIdentifiers)rel.Data).Count.ToString() } };
                }

                if (relLinks.Self != null || relLinks.Related != null)
                    rel.Links = relLinks;
            }
            return links;
        }

        public Dictionary<string, object> CreateLinkedResourceRepresentation(object objectGraph, IResourceMapping resourceMapping)
        {
            var objectDict = new Dictionary<string, object>();

            objectDict["id"] = resourceMapping.IdGetter(objectGraph).ToString();
            foreach (var propertyGetter in resourceMapping.PropertyGetters)
            {
                objectDict[propertyGetter.Key] = propertyGetter.Value(objectGraph);
            }

            if (resourceMapping.Relationships.Any())
            {
                var links = CreateLinks(objectGraph, resourceMapping);
                if (links.Any())
                {
                    objectDict["links"] = links;
                }
            }
            return objectDict;
        }

        public void AssureAllMappingsRegistered(Type type, Configuration config)
        {
            if (!config.IsMappingRegistered(type))
            {
                throw new MissingMappingException(type);
            }
        }

        public object GetCollection(JToken value, IRelationshipMapping mapping)
        {
            IList resultValue;

            if (!(value is JArray))
                throw new InvalidOperationException("Json array expected.");

            try
            {
                var array = (JArray)value;
                var listOfItems = array
                    .ToObject<List<string>>()
                    .Select(id =>
                    {
                        var obj = Activator.CreateInstance(mapping.ResourceMapping.ResourceRepresentationType);
                        mapping.ResourceMapping.IdSetter(obj, id);
                        return obj;
                    });

                resultValue = (IList)Activator.CreateInstance(mapping.RelatedCollectionProperty.PropertyType);
                foreach (var item in listOfItems)
                {
                    resultValue.Add(item);
                }
            }
            catch
            {
                resultValue = null;
            }
            return resultValue;
        }

        public object GetValue(JToken value, Type returnType)
        {
            object resultValue;
            try
            {
                if (returnType == typeof(string) && value.Value<string>() == null)
                {
                    resultValue = null;
                }
                else
                {
                    resultValue = Convert.ChangeType(value, returnType);
                }
            }
            catch
            {
                resultValue = default(ValueType);
            }
            return resultValue;
        }
    }
}