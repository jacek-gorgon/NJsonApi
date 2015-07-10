using System;
using System.Reflection;

namespace SocialCee.Framework.NJsonApi
{
    public interface ILinkMapping
    {
        string LinkName { get; set; }
        Type ParentType { get; set; }
        Type LinkedType { get; set; }
        string LinkedResourceType { get; set; }
        bool IsCollection { get; set; }
        PropertyInfo CollectionProperty { get; set; }
        bool SerializeAsLinked { get; set; }

        IResourceMapping ResourceMapping { get; set; }
        Func<object, object> Resource { get; }
        Func<object, object> ResourceId { get; }

        string ParentResourceNavigationPropertyName { get; }
        Type ParentResourceNavigationPropertyType { get; }
    }
}