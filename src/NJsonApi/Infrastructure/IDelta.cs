using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NJsonApi.Infrastructure
{
    internal interface IDelta<T> : IDelta
    {
        void AddFilter<TProperty>(params Expression<Func<T, TProperty>>[] filter);
        void Apply(T inputObject);
    }

    internal interface IDelta
    {
        Dictionary<string, object> ObjectPropertyValues { get; set; }
    }
}