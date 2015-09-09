using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApi.Common.Infrastructure;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations;
using NJsonApi.Serialization.Representations.Relationships;

namespace NJsonApi.Serialization
{
    public class JsonApiTransformer : IJsonApiTransformer
    {
        private JsonSerializer serializer;

        public TransformationHelper TransformationHelper { get; set; }

        public JsonSerializer Serializer
        {
            get { return serializer ?? (serializer = new JsonSerializer()); }
            set { serializer = value; }
        }

        public CompoundDocument Transform(object objectGraph, Context context)
        {
            Type innerObjectType = TransformationHelper.GetObjectType(objectGraph);

            if (objectGraph is HttpError)
            {
                return TransformationHelper.HandleHttpError(objectGraph as HttpError);
            }

            if (objectGraph is Exception)
            {
                return TransformationHelper.HandleException(objectGraph as Exception);
            }

            TransformationHelper.VerifyTypeSupport(innerObjectType);
            TransformationHelper.AssureAllMappingsRegistered(innerObjectType, context.Configuration);

            var result = new CompoundDocument
            {
                Meta = TransformationHelper.GetMetadata(objectGraph)
            };

            var resource = TransformationHelper.UnwrapResourceObject(objectGraph);
            var resourceMapping = context.Configuration.GetMapping(innerObjectType);

            var resourceList = TransformationHelper.UnifyObjectsToList(resource);
            var representationList = resourceList.Select(o => TransformationHelper.CreateResourceRepresentation(o, resourceMapping, context));
            var primaryResource = TransformationHelper.ChooseProperResourceRepresentation(resource, representationList);

            result.Data = primaryResource;

            if (resourceMapping.Relationships.Any())
            {
                result.Included = TransformationHelper.CreateIncludedRepresentations(resourceList, resourceMapping, context);
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
                    IRelationship irel;
                    updateDocument.Data.Relationships.TryGetValue(relMapping.RelationshipName, out irel);
                    Relationship rel = irel as Relationship;

                    if (rel == null)
                    {
                        continue;
                    }

                    if (relMapping.IsCollection && rel.Data is MultipleResourceIdentifiers)
                    {
                        var multipleIDs = (MultipleResourceIdentifiers)rel.Data;
                        var colProp = relMapping.RelatedCollectionProperty;

                        //relMapping.RelatedCollectionProperty RelatedCollectionProperty.GetValue();
                        
                        //if (property != null)
                        //{
                        //    var resultValue = TransformationHelper.GetCollection(value, relMapping);

                        //    string key = relMapping.RelationshipName.TrimStart('_');
                        //    delta.ObjectPropertyValues.Add(key, resultValue);
                        //}
                    }
                    else if (!relMapping.IsCollection && rel.Data is SingleResourceIdentifier)
                    {
                        delta.ObjectPropertyValues.Add(relMapping.ParentResourceNavigationPropertyName, TransformationHelper.GetValue(value, relMapping.ParentResourceNavigationPropertyType));
                    }
                    else
                        throw new InvalidOperationException("");
                }
            }

            return delta;
        }
    }

}