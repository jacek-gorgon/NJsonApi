
using JsonApiNet;
using JsonApiNet.Components;
using JsonApiNet.Resolvers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Documents;
namespace UtilJsonApiSerializer
{
    public class JsonApiSerializer : IJsonApiSerializer
    {

        public ConfigurationBuilder SerializerConfiguration { get; set; }
        public Dictionary<string, Type> ResolverSettings { get; set; }

        public JsonApiSerializer(Dictionary<string, Type> resolverSettings)
        {
            ResolverSettings = resolverSettings;
            SerializerConfiguration = new ConfigurationBuilder();
        }

        public object SerializeObject(ConfigurationBuilder serializerConfig, object obj)
        {
            var config = serializerConfig.Build();
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
