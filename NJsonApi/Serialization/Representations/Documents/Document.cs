using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Serialization.Documents
{
    public abstract class Document
    {
        public Dictionary<string, object> Metadata { get; set; }

    }
}
