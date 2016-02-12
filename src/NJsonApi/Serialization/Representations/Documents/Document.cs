using Newtonsoft.Json;
using System.Collections.Generic;

namespace NJsonApi.Serialization.Documents
{
    public abstract class Document
    {
        [JsonProperty(PropertyName = "meta", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Meta { get; set; }
    }
}
