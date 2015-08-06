using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApi.Serialization.Representations;

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
            Links = new Dictionary<string, LinkTemplate>();
            Included = new Dictionary<string, JToken>();
            Metadata = new Dictionary<string, object>();
            Errors = new Dictionary<string, Error>();
        }

        public IResourceRepresentation Data { get; set; }

        public Dictionary<string, LinkTemplate> Links { get; set; }

        public Dictionary<string, JToken> Included { get; set; }

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
