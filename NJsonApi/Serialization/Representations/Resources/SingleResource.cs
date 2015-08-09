using System.Collections.Generic;

namespace NJsonApi.Serialization.Representations
{
    public class SingleResource : IResourceRepresentation
    {
        public SingleResource()
        {
            Attributes = new Dictionary<string, object>();
            Relationships = new Dictionary<string, IRelationship>();
            Links = new Dictionary<string, ILink>();
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public Dictionary<string,object> Attributes { get; set; }
        public Dictionary<string, IRelationship> Relationships { get; set; }
        public Dictionary<string, ILink> Links { get; set; }
    }
}
