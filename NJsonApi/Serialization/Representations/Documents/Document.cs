using Newtonsoft.Json;
using System.Collections.Generic;

namespace NJsonApi.Serialization.Documents
{
    public abstract class Document
    {
        [JsonProperty(PropertyName = "metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }
    }
}
