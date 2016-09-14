using Newtonsoft.Json;
using System.Collections.Generic;

namespace UtilJsonApiSerializer.Serialization.Documents
{
    public abstract class Document
    {
        [JsonProperty(PropertyName = "meta", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Meta { get; set; }
    }
}
