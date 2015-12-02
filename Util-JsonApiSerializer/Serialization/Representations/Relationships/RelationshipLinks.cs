using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilJsonApiSerializer.Serialization.Representations.Relationships
{
    public class RelationshipLinks
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ILink self { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ILink related { get; set; }
    }
}
