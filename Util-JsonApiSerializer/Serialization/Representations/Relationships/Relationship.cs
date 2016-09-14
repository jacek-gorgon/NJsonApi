using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilJsonApiSerializer.Serialization.Representations.Relationships
{
    public class Relationship : IRelationship
    {
        [JsonProperty(PropertyName = "links", NullValueHandling=NullValueHandling.Ignore)]
        public RelationshipLinks Links { get; set; }

        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public IResourceLinkage Data { get; set; }

        [JsonProperty(PropertyName = "meta", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }
    }
}
