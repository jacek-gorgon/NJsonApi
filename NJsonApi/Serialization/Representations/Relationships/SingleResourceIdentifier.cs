using NJsonApi.Serialization.Representations.Resources;

namespace NJsonApi.Serialization.Representations.Relationships
{
    public class SingleResourceIdentifier : IResourceLinkage, IResourceIdentifier
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
