using Newtonsoft.Json;
using NJsonApi.Serialization.Representations.Resources;

namespace NJsonApi.Serialization.Representations.Relationships
{
    internal class SingleResourceIdentifier : IResourceLinkage, IResourceIdentifier
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }
}
