using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Serialization;

namespace UtilJsonApiSerializer.Serialization.Converters
{
    public class SerializationAwareConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ISerializationAware).IsAssignableFrom(objectType);
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ContractResolver= new CamelCasePropertyNamesContractResolver();
            ((ISerializationAware)value).Serialize(writer);
        }
    }
}
