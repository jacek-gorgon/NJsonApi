using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NJsonApi.Common.Utils;

namespace NJsonApi.Common.Infrastructure
{
    public class Delta<T> : IDelta<T> where T : new()
    {
        private readonly Dictionary<string, Action<T, object>> currentTypeSetters;
        private static Dictionary<string, Action<T, object>> typeSettersTemplates;

        private readonly Dictionary<string, CollectionInfo<T>> currentCollectionInfos;
        private static Dictionary<string, CollectionInfo<T>> collectionInfoTemplates;

        public Dictionary<string, object> ObjectPropertyValues { get; set; }
        public Dictionary<string, ICollectionDelta> CollectionDeltas { get; set; }

        public Delta()
        {
            if (typeSettersTemplates == null)
                typeSettersTemplates = ScanForProperties();
            if (collectionInfoTemplates == null)
                collectionInfoTemplates = ScanForCollections();

            currentTypeSetters = typeSettersTemplates.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            currentCollectionInfos = collectionInfoTemplates.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            ObjectPropertyValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            CollectionDeltas = new Dictionary<string, ICollectionDelta>();
        }

        public void FilterOut<TProperty>(params Expression<Func<T, TProperty>>[] filter)
        {
            foreach (var f in filter)
            {
                currentTypeSetters.Remove(f.GetPropertyInfo().Name);
                currentCollectionInfos.Remove(f.GetPropertyInfo().Name);
            }
        }

        public void SetValue<TProperty>(Expression<Func<T, TProperty>> property, object value)
        {
            var propertyInfo = property.GetPropertyInfo();
            ObjectPropertyValues[propertyInfo.Name] = value;
        }

        public TProperty GetValue<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyInfo = property.GetPropertyInfo();
            object val;
            ObjectPropertyValues.TryGetValue(propertyInfo.Name, out val);
            return (TProperty)val;
        }

        public void ApplySimpleProperties(T inputObject)
        {
            if (ObjectPropertyValues == null) return;
            foreach (var objectPropertyNameValue in ObjectPropertyValues)
            {
                Action<T, object> setter;
                currentTypeSetters.TryGetValue(objectPropertyNameValue.Key, out setter);
                if (setter != null)
                    setter(inputObject, objectPropertyNameValue.Value);
            }
        }

        public void ApplyCollections(T inputObject)
        {
            if (ObjectPropertyValues == null) return;
            foreach (var colDelta in CollectionDeltas)
            {
                CollectionInfo<T> info;
                currentCollectionInfos.TryGetValue(colDelta.Key, out info);
                if (info != null)
                {
                    var existingCollection = info.Getter(inputObject);
                    if (existingCollection == null)
                    {
                        existingCollection = Activator.CreateInstance(info.CollectionType) as ICollection;
                        info.Setter(inputObject, existingCollection);
                    }

                    colDelta.Value.Apply(existingCollection);
                }
            }
        }

        public ICollectionDelta<TElement> Collection<TElement>(Expression<Func<T, ICollection<TElement>>> collectionProperty)
        {
            ICollectionDelta delta;
            CollectionDeltas.TryGetValue(collectionProperty.GetPropertyInfo().Name, out delta);
            return delta as ICollectionDelta<TElement>;
        }

        public T ToObject()
        {
            var t = new T();
            ApplySimpleProperties(t);
            ApplyCollections(t);
            return t;
        }

        private Dictionary<string, Action<T, object>> ScanForProperties()
        {
            return typeof(T)
                .GetProperties()
                .Where(pi => !(typeof(ICollection).IsAssignableFrom(pi.PropertyType)))
                .ToDictionary(pi => pi.Name, pi => pi.ToCompiledSetterAction<T, object>());
        }

        private Dictionary<string, CollectionInfo<T>> ScanForCollections()
        {
            return typeof(T)
                .GetProperties()
                .Where(pi => (typeof(ICollection).IsAssignableFrom(pi.PropertyType)))
                .ToDictionary(pi => pi.Name, pi => new CollectionInfo<T>
                {
                    Getter = pi.ToCompiledGetterFunc<T, ICollection>(),
                    Setter = pi.ToCompiledSetterAction<T, ICollection>(),
                    CollectionType = pi.PropertyType,
                });
        }

        private class CollectionInfo<TOwner>
        {
            public Type CollectionType { get; set; }
            public Func<TOwner, ICollection> Getter { get; set; }
            public Action<TOwner, ICollection> Setter { get; set; }
        }
    }
}
