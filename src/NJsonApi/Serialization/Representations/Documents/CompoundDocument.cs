using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApi.Serialization.Representations;
using NJsonApi.Serialization.Representations.Resources;

namespace NJsonApi.Serialization.Documents
{
    internal class CompoundDocument
    {
        public CompoundDocument()
        {
            this.JsonApi = new JsonApi();
        }

        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public IResourceRepresentation Data { get; set; }

        [JsonProperty(PropertyName = "links", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ILink> Links { get; set; }

        [JsonProperty(PropertyName = "included", NullValueHandling = NullValueHandling.Ignore)]
        public List<SingleResource> Included { get; set; }

        [JsonProperty(PropertyName = "errors", NullValueHandling = NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; }

        [JsonProperty(PropertyName = "meta", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Meta { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> UnmappedAttributes { get; set; }

        [JsonProperty(PropertyName = "jsonapi", NullValueHandling = NullValueHandling.Ignore)]
        public JsonApi JsonApi { get; set; }
    }
}
