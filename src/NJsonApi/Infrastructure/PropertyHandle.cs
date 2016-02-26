using System;
using System.Linq.Expressions;
using NJsonApi.Utils;


namespace NJsonApi.Infrastructure
{
    public class PropertyHandle<TResource, TProperty> : IPropertyHandle<TResource, TProperty>
    {
        public PropertyHandle(Expression<Func<TResource, TProperty>> expression)
        {
            var pi = expression.GetPropertyInfo();
            Getter = pi.ToCompiledGetterFunc<TResource, TProperty>();
            Setter = pi.ToCompiledSetterAction<TResource, TProperty>();
            Name = pi.Name;
            Expression = expression;
        }

        public Expression<Func<TResource, TProperty>> Expression { get; private set; }
        public Func<TResource, TProperty> Getter { get; private set; }
        public Action<TResource, TProperty> Setter { get; private set; }
        public string Name { get; private set; }

        public Delegate GetterDelegate { get { return Getter; } }
        public Delegate SetterDelegate { get { return Setter; } }
    }

    public class PropertyHandle : IPropertyHandle
    {
        public PropertyHandle(LambdaExpression expression)
        {
            var pi = expression.GetPropertyInfo();
            GetterDelegate = pi.ToCompiledGetterDelegate(pi.DeclaringType, pi.PropertyType);
            SetterDelegate = pi.ToCompiledSetterDelegate(pi.DeclaringType, pi.PropertyType);
        }

        public Delegate GetterDelegate { get; private set; }

        public string Name { get; private set; }

        public Delegate SetterDelegate { get; private set; }
    }
}
