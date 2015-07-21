using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonApi.Serialization;

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

        public void Apply(HttpConfiguration configuration)
        {
            var serializer = GetJsonSerializer();
            var helper = new TransformationHelper();
            var transformer = new JsonApiTransformer { Serializer = serializer, TransformationHelper = helper };

            var filter = new JsonApiActionFilter(transformer, this);
            configuration.Filters.Add(filter);

            var formatter = new JsonApiFormatter(this, serializer);
            configuration.Formatters.Add(formatter);
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
