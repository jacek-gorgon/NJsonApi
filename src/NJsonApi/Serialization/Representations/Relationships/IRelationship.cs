using NJsonApi.Serialization.Representations.Relationships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Serialization.Representations
{
    internal interface IRelationship
    {
        RelationshipLinks Links { get; set; }

        IResourceLinkage Data { get; set; }

        Dictionary<string, string> Meta { get; set; }
    }
}
