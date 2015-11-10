

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

        public JsonApiSerializer()
        {
            SerializerConfiguration = new ConfigurationBuilder();
        }

        public object SerializeObject(ConfigurationBuilder serializerConfig, object obj)
        {
            var config = serializerConfig.Build();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };
            CompoundDocument result = sut.Transform(obj, new Context() { Configuration = config, RoutePrefix = string.Empty });

            return result;
        }




    }
}
