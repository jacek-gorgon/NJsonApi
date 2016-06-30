using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Serialization
{
    public class Error
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "links", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ILink> Links { get; set; }

        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "source.pointer", NullValueHandling = NullValueHandling.Ignore)]
        public string SourcePointer { get; set; }

        [JsonProperty(PropertyName = "source.parameter", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceParameter { get; set; }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "detail", NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }
   }
   
    
}