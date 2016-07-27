using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UtilJsonApiSerializer.Common.Infrastructure
{
    public interface IDelta<T> : IDelta
    {
        void AddFilter<TProperty>(params Expression<Func<T, TProperty>>[] filter);
        void Apply(T inputObject);
    }

    public interface IDelta
    {
        Dictionary<string, object> ObjectPropertyValues { get; set; }
    }
}