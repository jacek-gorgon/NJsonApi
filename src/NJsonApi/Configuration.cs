using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonApi.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Mvc.Formatters;
using System.Reflection;
using Microsoft.Net.Http.Headers;

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
            var transformer = new JsonApiTransformer(serializer);
            var actionFilter = new JsonApiActionFilter(transformer, this);
            var exceptionFilter = new JsonApiExceptionFilter(transformer);

            services.AddMvc(
                options =>
                    {
                        options.Filters.Add(actionFilter);
                        options.Filters.Add(exceptionFilter);
                        options.OutputFormatters.Insert(0, new JsonApiOutputFormatter());
                        options.InputFormatters.Insert(0, new JsonApiInputFormatter(serializer, this, transformer));
                    });
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
