using Newtonsoft.Json;

namespace SocialCee.Framework.NJsonApi.Serialization
{
    public class LinkTemplate
    {
        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}