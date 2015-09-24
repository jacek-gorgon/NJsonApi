using UtilJsonApiSerializer.Serialization.Representations.Resources;
using System.Collections.Generic;

namespace UtilJsonApiSerializer.Serialization.Representations
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
