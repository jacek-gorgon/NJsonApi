using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Test.Builders
{
    public class JsonSerializerBuilder
    {
        public static JsonSerializer Build()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            serializerSettings.Converters.Add(new StringEnumConverter() { CamelCaseText = true });
            serializerSettings.Formatting = Formatting.Indented;
            var jsonSerializer = JsonSerializer.Create(serializerSettings);
            return jsonSerializer;
        }
    }
}
