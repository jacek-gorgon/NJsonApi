using NJsonApi.Serialization.Representations;

namespace NJsonApi.Serialization
{
    internal interface ILinkBuilder
    {
        ILink FindLink(Context context, string id, IResourceMapping resourceMapping);
    }
}