using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UtilJsonApiSerializer.Utils
{
    public static class ExpressionUtils
    {
        public static PropertyInfo GetPropertyInfoFromExpression<TMain, TProperty>(Expression<Func<TMain, TProperty>> propertyExpression)
        {
            var expression = propertyExpression.Body;
            if (expression is UnaryExpression) 
                expression = ((UnaryExpression)expression).Operand;
            
            var me = expression as MemberExpression;

            if (me == null || !(me.Member is PropertyInfo))
                throw new NotSupportedException("Only simple property accessors are supported");

            return (PropertyInfo)me.Member;
        }

        public static Func<object, object> CompileToObjectTypedFunction<T>(Expression<Func<T, object>> expression)
        {
            ParameterExpression p = Expression.Parameter(typeof(object));
            Expression<Func<object, object>> convertedExpression = Expression.Lambda<Func<object, object>>
            (
                Expression.Invoke(expression, Expression.Convert(p, typeof(T))),
                p
            );

            return convertedExpression.Compile();
        }

        public static Expression<Action<object, object>> ConvertToObjectTypeExpression<T>(Expression<Action<T, object>> expression)
        {
            ParameterExpression p = Expression.Parameter(typeof(object));
            Expression<Action<object, object>> convertedExpression = Expression.Lambda<Action<object, object>>
            (
                Expression.Invoke(expression, Expression.Convert(p, typeof(T))),
                p
            );

            return convertedExpression;
        }

        public static Expression<Func<TResource, object>> CompileToObjectTypedExpression<TResource, TNested>(Expression<Func<TResource, TNested>> expression)
        {
            ParameterExpression p = Expression.Parameter(typeof(object));
            Expression<Func<TResource, object>> convertedExpression = Expression.Lambda<Func<TResource, object>>
            (
                Expression.Invoke(expression, Expression.Convert(p, typeof(TResource))),
                p
            );

            return convertedExpression;
        }
    }
}
