using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UtilJsonApiSerializer.Serialization.Representations.Resources;

namespace UtilJsonApiSerializer
{
    public interface IResourceMapping
    {
        Func<object, object> IdGetter { get; set; }
        Action<object, string> IdSetter { get; set; }
        Type ResourceRepresentationType { get; set; }
        string ResourceType { get; set; }
        string UrlTemplate { get; set; }
        List<IRelationshipMapping> Relationships { get; set; }
        Dictionary<string, Func<object, object>> PropertyGetters { get; set; }
        Dictionary<string, Action<object, object>> PropertySetters { get; }
        Dictionary<string, Expression<Action<object, object>>> PropertySettersExpressions { get; }
        ISerializerPipelineModule PipelineModule { get; set; }
        HashSet<string> RequestedFields { get; set; }
    }
}