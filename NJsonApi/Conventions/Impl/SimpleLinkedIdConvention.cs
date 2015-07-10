using System;
using System.Linq.Expressions;
using SocialCee.Framework.NJsonApi.Utils;

namespace SocialCee.Framework.NJsonApi.Conventions.Impl
{
    public class SimpleLinkedIdConvention : ILinkIdConvention
    {
        public Expression<Func<TMain, object>> GetIdExpression<TMain, TLinkedResource>(Expression<Func<TMain, TLinkedResource>> linkedResourceExpression)
        {
            var resourcePi = ExpressionUtils.GetPropertyInfoFromExpression(linkedResourceExpression);
            var idPropertyName = GetIdPropertyNameFromPropertyName(resourcePi.Name);
            var idPi = typeof(TMain).GetProperty(idPropertyName);
            if (idPi == null)
                return null;

            var parameterExp = Expression.Parameter(typeof(TMain));
            var propertyExp = Expression.Property(parameterExp, idPi);
            var castedExp = Expression.Convert(propertyExp, typeof(object));
            var lambda = Expression.Lambda<Func<TMain, object>>(castedExp, parameterExp);
            return lambda;
        }

        protected virtual string GetIdPropertyNameFromPropertyName(string propertyName)
        {
            return propertyName + "Id";
        }
    }
}
