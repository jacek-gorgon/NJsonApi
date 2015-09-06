using System;
using System.Linq.Expressions;
using NJsonApi.Utils;
using NJsonApi.Common.Utils;

namespace NJsonApi.Conventions.Impl
{
    public class CamelCaseLinkNameConvention : ILinkNameConvention
    {
        public virtual string GetLinkNameFromExpression<TResource, TLinkedResource>(Expression<Func<TResource, TLinkedResource>> propertyExpression)
        {
            var pi = propertyExpression.GetPropertyInfo();
            var name = CamelCaseUtil.ToCamelCase(pi.Name);
            return name;
        }
    }
}
