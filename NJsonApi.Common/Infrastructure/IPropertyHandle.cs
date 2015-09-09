using System;
using System.Linq.Expressions;

namespace NJsonApi.Common.Infrastructure
{
    public interface IPropertyHandle
    {
        Delegate GetterDelegate { get; }
        Delegate SetterDelegate { get; }
        string Name { get; }
    }

    public interface IPropertyHandle<TResource, TProperty> : IPropertyHandle
    {
        Expression<Func<TResource, TProperty>> Expression { get; }
        Func<TResource, TProperty> Getter { get; }
        Action<TResource, TProperty> Setter { get; }
    }
}