using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UtilJsonApiSerializer.Serialization.Representations.Resources;
using UtilJsonApiSerializer.Utils;

namespace UtilJsonApiSerializer
{
    public class ResourceMapping<T> : IResourceMapping
    {
        public Func<object, object> IdGetter { get; set; }
        public Action<object, string> IdSetter { get; set; }
        public Type ResourceRepresentationType { get; set; }
        public string ResourceType { get; set; }
        public string UrlTemplate { get; set; }
        public Dictionary<string, Func<object, object>> PropertyGetters { get; set; }
        public Dictionary<string, Action<object, object>> PropertySetters { get; private set; }
        public Dictionary<string, Expression<Action<object, object>>> PropertySettersExpressions { get; private set; }
        public Action<Type, SingleResource> CustomHandlerAction { get; set; }
        public List<IRelationshipMapping> Relationships { get; set; }

        public ResourceMapping()
        {
            ResourceRepresentationType = typeof(T);
            PropertyGetters = new Dictionary<string, Func<object, object>>();
            PropertySetters = new Dictionary<string, Action<object, object>>();
            PropertySettersExpressions = new Dictionary<string, Expression<Action<object, object>>>();
            Relationships = new List<IRelationshipMapping>();
        }
        public ResourceMapping(Expression<Func<T, object>> idPointer, string urlResource)
        {
            IdGetter = ExpressionUtils.CompileToObjectTypedFunction(idPointer);
            ResourceRepresentationType = typeof(T);
            UrlTemplate = urlResource;
            PropertyGetters = new Dictionary<string, Func<object, object>>();
            PropertySetters = new Dictionary<string, Action<object, object>>();
            PropertySettersExpressions = new Dictionary<string, Expression<Action<object, object>>>();
            Relationships = new List<IRelationshipMapping>();
        }

        public void AddPropertyGetter(string key, Expression<Func<T, object>> expression)
        {
            PropertyGetters.Add(key, ExpressionUtils.CompileToObjectTypedFunction(expression));
        }

        public void AddPropertySetter(string key, Expression<Action<T, object>> expression)
        {
            var convertedExpression = ExpressionUtils.ConvertToObjectTypeExpression(expression);

            PropertySettersExpressions.Add(key, convertedExpression);
            PropertySetters.Add(key, convertedExpression.Compile());
        }
    }
}
