using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NJsonApi.Serialization
{
    /// <summary>
    /// Represents a compound document, the root JSON API object returned.
    /// </summary>
    public class CompoundDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CompoundDocument()
        {
            Data = new Dictionary<string, object>();
            Links = new Dictionary<string, LinkTemplate>();
            Linked = new Dictionary<string, JToken>();
            Metadata = new Dictionary<string, object>();
            Errors = new Dictionary<string, Error>();
        }

        public Dictionary<string, object> Data { get; set; }

        public Dictionary<string, LinkTemplate> Links { get; set; }

        public Dictionary<string, JToken> Linked { get; set; }

        /// <summary>
        /// TODO: support metadata
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// TODO: support errors
        /// </summary>
        public Dictionary<string, Error> Errors { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> UnmappedAttributes { get; set; }

        public string GetPrimaryResourceHref()
        {
            var resource = Data.FirstOrDefault().Value as Dictionary<string, object>;
            if (resource != null && resource.ContainsKey("href"))
            {
                return resource["href"].ToString();
            }

            return string.Empty;
        }
    }
}
