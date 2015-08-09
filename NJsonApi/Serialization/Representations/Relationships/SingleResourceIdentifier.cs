using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Serialization.Representations.Relationships
{
    public class SingleResourceIdentifier : IResourceLinkage
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
