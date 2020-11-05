using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Documents;
namespace UtilJsonApiSerializer
{
    public class JsonApiSerializer : IJsonApiSerializer
    {
        private readonly string _routePrefix;

        public ConfigurationBuilder SerializerConfiguration { get; set; }

        public JsonApiSerializer(string routePrefix = "")
        {
            SerializerConfiguration = new ConfigurationBuilder();
            _routePrefix = routePrefix;
        }

        public object SerializeObject(ConfigurationBuilder serializerConfig, object obj)
        {
            var config = serializerConfig.Build();
            RunPreSerializationPipelineModules(config, obj);
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };
            CompoundDocument result = sut.Transform(obj, new Context() { Configuration = config, RoutePrefix = _routePrefix });

            return result;
        }

        private void RunPreSerializationPipelineModules(Configuration config, object objectData)
        {
            var objectType = TransformationHelper.GetObjectType(objectData);
            var preSerializerPipelineModule = config.GetPreSerializerPipelineModule(objectType);
            preSerializerPipelineModule.Run(objectData);
        }


    }
}
