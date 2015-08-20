using Newtonsoft.Json;
using NJsonApi.Serialization.Converters;

namespace NJsonApi.Serialization.Representations
{
    [JsonConverter(typeof(SerializationAwareConverter))]
    public class SimpleLink : ILink, ISerializationAware
    {
        public string Href { get; set; }
        public void Serialize(JsonWriter writer)
        {
            writer.WriteValue(Href);
        }
        public override string ToString()
        {
            return Href;
        }
    }
}
