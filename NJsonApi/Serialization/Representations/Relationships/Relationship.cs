using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Serialization.Representations.Relationships
{
    public class Relationship : IRelationship
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public RelationshipLinks Links { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IResourceLinkage Data { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }
    }
}
