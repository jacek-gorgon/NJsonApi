
using System;
using System.Reflection;

namespace UtilJsonApiSerializer
{
    public interface IRelationshipMapping
    {
        string RelationshipName { get; set; }
        Type ParentType { get; set; }
        Type RelatedBaseType { get; set; }
        string RelatedBaseResourceType { get; set; }
        bool IsCollection { get; set; }
        ResourceInclusionRules InclusionRule { get; set; }

        IResourceMapping ResourceMapping { get; set; }
        PropertyInfo RelatedCollectionProperty { get; set; }
        Func<object, object> RelatedResource { get; }
        Func<object, object> RelatedResourceId { get; }

        string SelfUrlTemplate { get; set; }
        string RelatedUrlTemplate { get; set; }

        string ParentResourceNavigationPropertyName { get; }
        Type ParentResourceNavigationPropertyType { get; }
    }

    public enum ResourceInclusionRules
    {
        /// <summary>
        /// For to-one relationships, the related resource will be included unless it's reference is null and simultaneously it's ID is not null.
        /// For to-many relationships, the related resources will be included only if the collection instance is not null.
        /// Recommended rule.
        /// </summary>
        Smart,
        /// <summary>
        /// The related resource(s) will not be included.
        /// Use if precisely controlling the returned object graph is inconvenient or impossible.
        /// </summary>
        ForceOmit,
        /// <summary>
        /// The related resource(s) will always be included.
        /// With this option, the serializer assumes related instances are loaded correctly and any missing instance will result in null being serialized.
        /// Not recommended for to-one relationships, since forgeting to load the related resource may lead to inconsistent link vs. resource linkage output.
        /// </summary>
        ForceInclude
    }
}