using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

namespace NJsonApi.Serialization
{
    internal class JsonApiTransformer : IJsonApiTransformer
    {
        private JsonSerializer serializer;

        private readonly TransformationHelper transformationHelper = new TransformationHelper();

        public JsonApiTransformer()
        { }

        public JsonApiTransformer(JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        public JsonSerializer Serializer
        {
            get { return serializer ?? (serializer = new JsonSerializer() ); }
            set { serializer = value; }
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
            Type innerObjectType = transformationHelper.GetObjectType(objectGraph);

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
            var primaryResource = transformationHelper.ChooseProperResourceRepresentation(resource, representationList);

            result.Data = primaryResource;

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

            var resourceKey = "data";
            if (!updateDocument.Data.ContainsKey(resourceKey))
            {
                return delta;
            }

            var jsonApiDocument = updateDocument.Data[resourceKey] as JObject;
            if (jsonApiDocument == null)
            {
                return delta;
            }

            var resource = jsonApiDocument["attributes"] as JObject;

            // Scan the data for which properties are only set
            foreach (var propertySetter in mapping.PropertySettersExpressions)
            {
                JToken value;
                resource.TryGetValue(propertySetter.Key, StringComparison.CurrentCultureIgnoreCase, out value);
                if (value == null)
                {
                    continue;
                }
                // Set only the properties that are present
                var methodCallExpression = propertySetter.Value.Body as MethodCallExpression;
                if (methodCallExpression != null)
                {
                    Type returnType = methodCallExpression.Arguments[0].Type;

                    var resultValue = transformationHelper.GetValue(value, returnType);

                    string key = propertySetter.Key.TrimStart('_');
                    delta.ObjectPropertyValues.Add(key, resultValue);
                }
            }

            JToken linksToken;
            jsonApiDocument.TryGetValue("links", StringComparison.CurrentCultureIgnoreCase, out linksToken);
            JObject links = linksToken as JObject;

            if (links != null)
            {
                foreach (var link in mapping.Relationships)
                {
                    JToken value;
                    links.TryGetValue(link.RelationshipName, StringComparison.CurrentCultureIgnoreCase, out value);
                    if (value == null)
                    {
                        continue;
                    }

                    if (link.IsCollection)
                    {
                        var property = link.RelatedCollectionProperty;
                        if (property != null)
                        {
                            var resultValue = transformationHelper.GetCollection(value, link);

                            string key = link.RelationshipName.TrimStart('_');
                            delta.ObjectPropertyValues.Add(key, resultValue);
                        }    
                    }
                    else
                    {
                        delta.ObjectPropertyValues.Add(link.ParentResourceNavigationPropertyName, transformationHelper.GetValue(value, link.ParentResourceNavigationPropertyType));
                    }
                }
            }

            return delta;
        }
    }

}