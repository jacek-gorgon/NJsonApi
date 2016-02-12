using Newtonsoft.Json;
using NJsonApi.Serialization.Representations.Resources;
using System.Collections.Generic;

namespace NJsonApi.Serialization.Representations.Resources
{
    public class SingleResource : IResourceRepresentation, IResourceIdentifier
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "attributes", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Attributes { get; set; }

        [JsonProperty(PropertyName = "relationships", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, IRelationship> Relationships { get; set; }

        [JsonProperty(PropertyName = "links", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ILink> Links { get; set; }
    }
}
