using NJsonApi.Serialization.Representations.Relationships;
using System.Collections.Generic;

namespace NJsonApi.Serialization.Representations
{
    internal class MultipleResourceIdentifiers : List<SingleResourceIdentifier>, IResourceLinkage
    {
        public MultipleResourceIdentifiers()
        {
        }

        public MultipleResourceIdentifiers(IEnumerable<SingleResourceIdentifier> c) : base(c)
        {
        }
    }
}
