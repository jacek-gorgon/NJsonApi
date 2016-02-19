using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Serialization.Representations.Relationships
{
    internal class RelationshipLinks
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ILink Self { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ILink Related { get; set; }
    }
}
