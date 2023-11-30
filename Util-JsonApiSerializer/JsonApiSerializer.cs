#if NETCOREAPP
    using Microsoft.AspNetCore.Http;
#else
#endif
using System.Collections.Generic;
using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Documents;
namespace UtilJsonApiSerializer
{
    public class JsonApiSerializer : IJsonApiSerializer
    {
        private readonly string _routePrefix;

        public ConfigurationBuilder SerializerConfiguration { get; set; }
// In Net Core we do not have access to the Http Context directly, so we need to inject it via httpContextAccessor
#if NETCOREAPP
        private IHttpContextAccessor _accessor;
        public JsonApiSerializer(IHttpContextAccessor accessor, string routePrefix = "")
        {
            SerializerConfiguration = new ConfigurationBuilder();
            _routePrefix = routePrefix;
            _accessor = accessor;
        }
#else

        public JsonApiSerializer(string routePrefix = "")
        {
            SerializerConfiguration = new ConfigurationBuilder();
            _routePrefix = routePrefix;
        }
#endif
        public object SerializeObject(ConfigurationBuilder serializerConfig, object obj)
        {
            var config = serializerConfig.Build();
            RunPreSerializationPipelineModules(config, obj);
#if NETCOREAPP
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper(_accessor) };

#else
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };
#endif
            CompoundDocument result = sut.Transform(obj, new Context() { Configuration = config, RoutePrefix = _routePrefix });

            return result;
        }

        private void RunPreSerializationPipelineModules(Configuration config, object objectData)
        {
            var objectType = TransformationHelper.GetObjectType(objectData);
            var preSerializerPipelineModule = config.GetPreSerializerPipelineModule(objectType);
            if (preSerializerPipelineModule != null)
            {
                if (objectData is IEnumerable<object> enumerableData)
                {
                    foreach (var item in enumerableData)
                    {
                        preSerializerPipelineModule.Run(item);
                    }
                }
                else
                {
                    preSerializerPipelineModule.Run(objectData);
                }
                
            }
        }


    }
}
