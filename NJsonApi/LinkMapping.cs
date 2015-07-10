using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SocialCee.Framework.NJsonApi
{
    public class LinkMapping<TParent, TNested> : ILinkMapping
        where TNested : class
    {
        public string LinkName { get; set; }
        public Type ParentType { get; set; }
        public Type LinkedType { get; set; }
        public string LinkedResourceType { get; set; }
        public IResourceMapping ResourceMapping { get; set; }
        public bool IsCollection { get; set; }
        public PropertyInfo CollectionProperty{ get; set; }
        public bool SerializeAsLinked { get; set; }
        public string ParentResourceNavigationPropertyName { get; private set; }
        public Type ParentResourceNavigationPropertyType { get; private set; }

        public Func<object, object> Resource { get; private set; }
        public Expression<Func<TParent, object>> ResourceGetter
        {
            set { Resource = CompileToObjectTypedFunction(value); }
        }

        public Func<object, object> ResourceId { get; private set; }

        public Expression<Func<TParent, object>> ResourceIdGetter
        {
            set
            {
                ResourceId = CompileToObjectTypedFunction(value);
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
            LinkedType = typeof(TNested);
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
