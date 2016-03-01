using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations;
using NJsonApi.Utils;
using NJsonApi.Serialization.Representations.Relationships;
using Microsoft.AspNet.Mvc.ApiExplorer;

namespace NJsonApi.Serialization
{
    internal class JsonApiTransformer : IJsonApiTransformer
    {
        private JsonSerializer serializer;

        private readonly TransformationHelper transformationHelper = new TransformationHelper();
        private readonly IApiDescriptionGroupCollectionProvider descriptionProvider;

        internal JsonApiTransformer()
        { }

        public JsonApiTransformer(JsonSerializer serializer, IApiDescriptionGroupCollectionProvider descriptionProvider)
        {
            this.serializer = serializer;
            this.descriptionProvider = descriptionProvider;
        }

        public CompoundDocument Transform(Exception e, int httpStatus)
        {
            var result = new CompoundDocument();
            result.Errors = new List<Error>()
            {
                new Error()
                {
                    Title = "There has been an unhandled error when processing your request.",
                    Detail = e.Message,
                    Code = e.ToString(),
                    Status = httpStatus
                }
            };

            return result;
        }

        public CompoundDocument Transform(object objectGraph, Context context)
        {
            Type innerObjectType = Reflection.GetObjectType(objectGraph);

            transformationHelper.VerifyTypeSupport(innerObjectType);
            transformationHelper.AssureAllMappingsRegistered(innerObjectType, context.Configuration);

            var result = new CompoundDocument
            {
                Meta = transformationHelper.GetMetadata(objectGraph)
            };

            var resource = transformationHelper.UnwrapResourceObject(objectGraph);
            var resourceMapping = context.Configuration.GetMapping(innerObjectType);

            var resourceList = transformationHelper.UnifyObjectsToList(resource);
            var representationList = resourceList.Select(o => transformationHelper.CreateResourceRepresentation(o, resourceMapping, context));
            result.Data = transformationHelper.ChooseProperResourceRepresentation(resource, representationList);
            result.Links = transformationHelper.GetTopLevelLinks(context.RequestUri);

            if (resourceMapping.Relationships.Any())
            {
                result.Included = transformationHelper.CreateIncludedRepresentations(resourceList, resourceMapping, context);
            }

            return result;
        }

        public IDelta TransformBack(UpdateDocument updateDocument, Type type, Context context)
        {
            var mapping = context.Configuration.GetMapping(type);
            var openGeneric = typeof(Delta<>);
            var closedGenericType = openGeneric.MakeGenericType(type);
            var delta = Activator.CreateInstance(closedGenericType) as IDelta;

            if (delta == null)
            {
                return null;
            }

            // Scan the data for which properties are only set
            foreach (var propertySetter in mapping.PropertySettersExpressions)
            {
                object value;
                updateDocument.Data.Attributes.TryGetValue(propertySetter.Key, out value);
                if (value != null)
                    delta.ObjectPropertyValues.Add(propertySetter.Key, value);
            }

            if (updateDocument.Data.Relationships != null)
            {
                foreach (var relMapping in mapping.Relationships)
                {
                    var relatedTypeMapping = context.Configuration.GetMapping(relMapping.RelatedBaseType);
                    var relationship = updateDocument.Data.Relationships[relMapping.RelationshipName];
                    if (relationship == null)
                    {
                        continue;
                    }

                    if (relMapping.IsCollection && relationship.Data is MultipleResourceIdentifiers)
                    {
                        var multipleIDs = (MultipleResourceIdentifiers)relationship.Data;
                        var openGenericCollection = typeof(CollectionDelta<>);
                        var closedGenericTypeCollection = openGenericCollection.MakeGenericType(relMapping.RelatedBaseType);
                        var collection = Activator.CreateInstance(closedGenericTypeCollection, relatedTypeMapping.IdGetter) as ICollectionDelta;
                        var openGenericList = typeof(List<>);
                        var closedGenericTypeList = openGenericList.MakeGenericType(relMapping.RelatedBaseType);
                        collection.Elements = Activator.CreateInstance(closedGenericTypeList) as IList;
                        foreach (var id in multipleIDs)
                        {
                            var colProp = relMapping.RelatedCollectionProperty;

                            var newInstance = Activator.CreateInstance(relMapping.RelatedBaseType);
                            relatedTypeMapping.IdSetter(newInstance, id.Id);
                            (collection.Elements as IList).Add(newInstance);
                        }
                        delta.CollectionDeltas.Add(relMapping.RelationshipName, collection);

                    }
                    else if (!relMapping.IsCollection && relationship.Data is SingleResourceIdentifier)
                    {
                        var singleId = relationship.Data as SingleResourceIdentifier;
                        var instance = Activator.CreateInstance(relMapping.RelatedBaseType);
                        relatedTypeMapping.IdSetter(instance, singleId.Id);
                        delta.ObjectPropertyValues.Add(relMapping.RelationshipName, instance);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

            }
            return delta;
        }
    }
}