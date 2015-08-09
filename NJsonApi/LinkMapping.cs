using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NJsonApi
{
    public class LinkMapping<TParent, TNested> : IRelationshipMapping
        where TNested : class
    {
        public string RelationshipName { get; set; }
        public Type ParentType { get; set; }
        public Type RelatedBaseType { get; set; }
        public string RelatedBaseResourceType { get; set; }
        public IResourceMapping ResourceMapping { get; set; }
        public string SelfUrlTemplate { get; set; }
        public string RelatedUrlTemplate { get; set; }
        public bool IsCollection { get; set; }
        public PropertyInfo RelatedCollectionProperty{ get; set; }
        public ResourceInclusionRules InclusionRule { get; set; }
        public string ParentResourceNavigationPropertyName { get; private set; }
        public Type ParentResourceNavigationPropertyType { get; private set; }

        public Func<object, object> RelatedResource { get; private set; }
        public Expression<Func<TParent, object>> ResourceGetter
        {
            set { RelatedResource = CompileToObjectTypedFunction(value); }
        }

        public Func<object, object> RelatedResourceId { get; private set; }

        public Expression<Func<TParent, object>> ResourceIdGetter
        {
            set
            {
                RelatedResourceId = CompileToObjectTypedFunction(value);
                ParentResourceNavigationPropertyName = GetPropertyName(value);
                ParentResourceNavigationPropertyType = GetPropertyType(value);
            }
        }

        private string GetPropertyName(Expression<Func<TParent, object>> value)
        {
            if (value == null)
            {
                return null;
            }

            var body = value.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression) value.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body != null)
            {
                 return body.Member.Name;
            }

            return null;
        }

        private Type GetPropertyType(Expression<Func<TParent, object>> value)
        {
            if (value == null)
            {
                return null;
            }

            var body = value.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression)value.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body != null)
            {
                return body.Type;
            }

            return null;
        }

        public LinkMapping()
        {
            ParentType = typeof(TParent);
            RelatedBaseType = typeof(TNested);
        }

        private Type GetItemType(Type ienumerableType)
        {
            return ienumerableType
                .GetInterfaces()
                .Where(t => t.IsGenericType == true && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetGenericArguments()[0])
                .SingleOrDefault();
        }

        private Func<object, object> CompileToObjectTypedFunction(Expression<Func<TParent, object>> expression)
        {
            if (expression == null)
            {
                return null;
            }

            ParameterExpression p = Expression.Parameter(typeof(object));
            Expression<Func<object, object>> convertedExpression = Expression.Lambda<Func<object, object>>
            (
                Expression.Invoke(expression, Expression.Convert(p, typeof(TParent))),
                p
            );

            return convertedExpression.Compile();
        }

        

    }
}
