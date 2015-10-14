using JsonApiNet;
using JsonApiNet.Components;
using JsonApiNet.Resolvers;
using System;
using System.Collections.Generic;
using UtilJsonApiSerializer;
using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Documents;
using System.Linq;

namespace UtilJsonApiSerializer
{
    public class JsonApiSerializer
    {

        public ConfigurationBuilder SerializerConfiguration { get; set; }
        public Dictionary<string,Type> ResolverSettings { get; set; }

        public JsonApiSerializer(Dictionary<string, Type> resolverSettings) {
            ResolverSettings = resolverSettings;
            SerializerConfiguration = new ConfigurationBuilder();
        }

        public ConfigurationBuilder BuildResource<T>(string includedfields, string includedrelationships)
        {

            var fields = new List<string>();
            var includes = new List<string>();

            //include fields
            if (!string.IsNullOrEmpty(includedfields))
            {
                fields.AddRange(includedfields.Split(','));
            }

            //include relationships
            if (!string.IsNullOrEmpty(includedrelationships))
            {
                includes.AddRange(includedrelationships.Split(','));
            }

            SerializerConfiguration.Resource<T>()
             .WithSpecifiedSimpleProperties(fields)
             .WithSpecifiedLinkedResources(includes)
             .WithLinkTemplate("/{type}/{id}");

            return SerializerConfiguration;
        }

        public object SerializeObject(object obj)
        {

            var config = SerializerConfiguration.Build();

            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };
            CompoundDocument result = sut.Transform(obj, new Context() { Configuration = config, RoutePrefix = string.Empty });

            return result;

        }

        public T DeserializeObject<T>(string json)
        {
            var resolver = new StringToTypeResolver(ResolverSettings);
            return JsonApi.ResourceFromDocument<T>(json, resolver);
        }

        public Dictionary<string, object> GetDocumentProperties(string json)
        {
             var resolver = new StringToTypeResolver(ResolverSettings);
            return JsonApi.Document<JsonApiDocument>(json, resolver).Data[0].Attributes;
        }


   
    }
}
