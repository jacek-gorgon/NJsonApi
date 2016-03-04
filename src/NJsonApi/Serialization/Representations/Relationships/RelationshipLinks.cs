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
        [JsonProperty(PropertyName = "self", NullValueHandling =NullValueHandling.Ignore)]
        public ILink Self { get; set; }

        [JsonProperty(PropertyName = "related", NullValueHandling =NullValueHandling.Ignore)]
        public ILink Related { get; set; }
    }
}
