using System;
using Newtonsoft.Json;

namespace NJsonApi.Infrastructure
{
    public class Patch
    {
        public PatchOperation Operation { get; set; }
        public Object Content { get; set; }
        public DateTime EventTime { get; set; }
    }

    public enum PatchOperation
    {
        [JsonProperty(PropertyName = "add")]
        Add,
        [JsonProperty(PropertyName = "remove")]
        Remove,
        [JsonProperty(PropertyName = "replace")]
        Replace
    }
}

