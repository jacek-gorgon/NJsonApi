using Newtonsoft.Json;
using NJsonApi.Serialization.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Serialization.Representations
{
    [JsonConverter(typeof(SerializationAwareConverter))]
    public class SimpleLink : ILink, ISerializationAware
    {
        public string Href { get; set; }
        public string Serialize()
        {
            return Href;
        }
    }
}
