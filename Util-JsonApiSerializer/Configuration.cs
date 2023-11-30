using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
#else
using System.Web.Http;
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UtilJsonApiSerializer.Serialization;

namespace UtilJsonApiSerializer
{
    public class Configuration
    {
        private readonly Dictionary<string, IResourceMapping> resourcesMappingsByResourceType = new Dictionary<string, IResourceMapping>();
        private readonly Dictionary<Type, IResourceMapping> resourcesMappingsByType = new Dictionary<Type, IResourceMapping>();
        private readonly Dictionary<Type, IPreSerializerPipelineModule> _preSerializerPipelineModules = new Dictionary<Type, IPreSerializerPipelineModule>();

        public void AddMapping(IResourceMapping resourceMapping)
        {
            resourcesMappingsByResourceType[resourceMapping.ResourceType] = resourceMapping;
            resourcesMappingsByType[resourceMapping.ResourceRepresentationType] = resourceMapping;
        }

        public bool IsMappingRegistered(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                return resourcesMappingsByType.ContainsKey(type.GetGenericArguments()[0]);
            }

            return resourcesMappingsByType.ContainsKey(type);
        }

        public IResourceMapping GetMapping(Type type)
        {
            IResourceMapping mapping;
            resourcesMappingsByType.TryGetValue(type, out mapping);
            return mapping;
        }

        public IPreSerializerPipelineModule GetPreSerializerPipelineModule(Type type)
        {
            _preSerializerPipelineModules.TryGetValue(type, out var preSerializerPipelineModule);
            return preSerializerPipelineModule;
        }

#if NETCOREAPP
        public void Apply(IServiceCollection services, IHttpContextAccessor accessor)
        {
            var serializer = GetJsonSerializer();
            var inputFormatter = new JsonApiInputFormatter(this, serializer);
            var outputFormatter = new JsonApiOutputFormatter(this, serializer);

           
            services.AddControllers(options =>
            {
                options.InputFormatters.Add(inputFormatter);
                options.OutputFormatters.Add(outputFormatter);
            });
            var helper = new TransformationHelper(accessor);

            var transformer = new JsonApiTransformer { Serializer = serializer, TransformationHelper = helper };
            var filter = new JsonApiActionFilter(transformer, this);
            services.AddMvc(options =>
            {
                options.Filters.Add(filter);
            });
        }
#else
        public void Apply(HttpConfiguration configuration)
        {
            // var conf = new ConfigurationBuilder();


            var serializer = GetJsonSerializer();
            var helper = new TransformationHelper();
            var transformer = new JsonApiTransformer { Serializer = serializer, TransformationHelper = helper };

            var filter = new JsonApiActionFilter(transformer, this);
            configuration.Filters.Add(filter);

            var formatter = new JsonApiFormatter(this, serializer);
            configuration.Formatters.Add(formatter);

        }
#endif
        private static JsonSerializer GetJsonSerializer()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            serializerSettings.Converters.Add(new StringEnumConverter() { CamelCaseText = true });
#if DEBUG
            serializerSettings.Formatting = Formatting.Indented;
#endif
            var jsonSerializer = JsonSerializer.Create(serializerSettings);
            return jsonSerializer;
        }

        public void AddPreSerializationModule(Type type, IPreSerializerPipelineModule preSerializerPipelineModule)
        {
            _preSerializerPipelineModules.Add(type, preSerializerPipelineModule);
        }
    }
}
