using NJsonApi.Serialization.Representations;

namespace NJsonApi.Serialization
{
    internal interface ILinkBuilder
    {
        ILink FindResourceSelfLink(Context context, string id, IResourceMapping resourceMapping);
    }
}