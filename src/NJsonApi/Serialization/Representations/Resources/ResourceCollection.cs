using NJsonApi.Serialization.Representations.Resources;
using System.Collections.Generic;

namespace NJsonApi.Serialization.Representations
{
    internal class ResourceCollection : List<SingleResource>, IResourceRepresentation
    {
        public ResourceCollection()
        {
        }

        public ResourceCollection(IEnumerable<SingleResource> list) : base(list)
        {
        }
    }
}
