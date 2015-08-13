using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApi.Serialization.Representations;
using NJsonApi.Serialization.Representations.Resources;

namespace NJsonApi.Serialization.Documents
{
    /// <summary>
    /// Represents a compound document, the root JSON API object returned.
    /// </summary>
    public class CompoundDocument : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CompoundDocument()
        {
        }

        public IResourceRepresentation Data { get; set; }

        public Dictionary<string, LinkTemplate> Links { get; set; }

        public List<SingleResource> Included { get; set; }

        public Dictionary<string, Error> Errors { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> UnmappedAttributes { get; set; }

        public string GetPrimaryResourceHref()
        {
            var resource = Data as Dictionary<string, object>;
            if (resource != null && resource.ContainsKey("href"))
            {
                return resource["href"].ToString();
            }

            var resourceList = Data as List<Dictionary<string, object>>;
            var href = resourceList?.FirstOrDefault()?.FirstOrDefault(kvp => kvp.Key == "href").Value as string;

            return href ?? string.Empty;
        }
    }
}
