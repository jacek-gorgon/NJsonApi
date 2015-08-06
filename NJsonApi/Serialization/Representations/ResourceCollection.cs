using System.Collections.Generic;

namespace NJsonApi.Serialization.Representations
{
    public class ResourceCollection : List<SingleResource>, IResourceRepresentation
    {
        public ResourceCollection()
        {
        }

        public ResourceCollection(IEnumerable<SingleResource> list) : base(list)
        {
        }
    }
}
