using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UtilJsonApiSerializer.Common.Infrastructure
{
    public class Delta<T> : IDelta<T> where T : new()
    {
        private readonly Dictionary<string, Action<object, object>> currentTypeSetters;
        // ReSharper disable once StaticFieldInGenericType
        private static Dictionary<string, Action<object, object>> typeSettersTemplate;
        public Dictionary<string, object> ObjectPropertyValues { get; set; }

        public Delta()
        {
            if (typeSettersTemplate == null)
                typeSettersTemplate = ScanForProperties();
            currentTypeSetters = typeSettersTemplate.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            ObjectPropertyValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddFilter<TProperty>(params Expression<Func<T, TProperty>>[] filter)
        {
            foreach (var f in filter)
            {
                currentTypeSetters.Remove(GetPropertyInfoFromExpression(f).Name);
            }
        }

        public void SetValue<TProperty>(Expression<Func<T, TProperty>> property, object value)
        {
            var propertyInfo = GetPropertyInfoFromExpression(property);
            ObjectPropertyValues[propertyInfo.Name] = value;
        }

        public TProperty GetValue<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyInfo = GetPropertyInfoFromExpression(property);
            object val;
            ObjectPropertyValues.TryGetValue(propertyInfo.Name, out val);
            return (TProperty)val;
        }

        private PropertyInfo GetPropertyInfoFromExpression<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var me = propertyExpression.Body as MemberExpression;
            if (me == null || !(me.Member is PropertyInfo))
                throw new NotSupportedException("Only simple property accessors are supported");
            return (PropertyInfo)me.Member;
        }

        public void Apply(T inputObject)
        {
            const BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

            if (ObjectPropertyValues == null) return;
            foreach (var objectPropertyNameValue in ObjectPropertyValues)
            {
                if (currentTypeSetters.Keys.Select(k => k.ToLower()).Contains(objectPropertyNameValue.Key.ToLower()))
                {
                    typeof(T).GetProperty(objectPropertyNameValue.Key, bindingFlags).SetValue(inputObject, objectPropertyNameValue.Value);
                }
            }
        }

        public T ToObject()
        {
            var t = new T();
            Apply(t);
            return t;
        }

        public Dictionary<string, Action<object, object>> ScanForProperties()
        {
            return typeof(T)
                .GetProperties()
                .ToDictionary(pi => pi.Name, pi => (Action<object, object>)pi.SetValue);
        }
    }
}
