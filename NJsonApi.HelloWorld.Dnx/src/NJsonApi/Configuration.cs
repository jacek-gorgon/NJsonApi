using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonApi.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Mvc.Formatters;
using System.Reflection;

namespace NJsonApi
{
    public class Configuration
    {
        private readonly Dictionary<string, IResourceMapping> resourcesMappingsByResourceType = new Dictionary<string, IResourceMapping>();
        private readonly Dictionary<Type, IResourceMapping> resourcesMappingsByType = new Dictionary<Type, IResourceMapping>();

        public void AddMapping(IResourceMapping resourceMapping)
        {
            resourcesMappingsByResourceType[resourceMapping.ResourceType] = resourceMapping;
            resourcesMappingsByType[resourceMapping.ResourceRepresentationType] = resourceMapping;
        }

        public bool IsMappingRegistered(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) && type.GetTypeInfo().IsGenericType)
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

        public void Apply(IServiceCollection services)
        {
            var serializer = GetJsonSerializer();
            var helper = new TransformationHelper();
            var transformer = new JsonApiTransformer { Serializer = serializer, TransformationHelper = helper };

            var filter = new JsonApiActionFilter(transformer, this);

            services.AddMvc(
                config =>
                    {
                        config.Filters.Add(filter);
                        config.OutputFormatters.Insert(0, GetJsonOutputFormatter());

                        // TODO Input formatter required for the complex Delta binding
                    });
        }

        private static IOutputFormatter GetJsonOutputFormatter()
        {
            var jsonOutputFormatter = new JsonOutputFormatter();
            return jsonOutputFormatter;
        }

        private static JsonSerializer GetJsonSerializer()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            serializerSettings.Converters.Add(new StringEnumConverter() { CamelCaseText = true});
#if DEBUG
            serializerSettings.Formatting = Formatting.Indented;
#endif
            var jsonSerializer = JsonSerializer.Create(serializerSettings);
            return jsonSerializer;
        }
    }
}
