using Newtonsoft.Json;
using NJsonApi.Serialization.Converters;
using System;

namespace NJsonApi.Serialization.Representations
{
    [JsonConverter(typeof(SerializationAwareConverter))]
    internal class SimpleLink : ILink, ISerializationAware
    {
        public SimpleLink()
        {

        }

        public SimpleLink(Uri href)
        {
            this.Href = href.AbsoluteUri;
        }

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
