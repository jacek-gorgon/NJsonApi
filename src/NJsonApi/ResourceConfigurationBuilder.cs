using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NJsonApi.Conventions;
using NJsonApi.Utils;
using NJsonApi.Infrastructure;

namespace NJsonApi
{
    public class ResourceConfigurationBuilder<TResource, TController> : IResourceConfigurationBuilder
    {
        public IResourceMapping BuiltResourceMapping { get; set; }
        public IResourceTypeConvention ResourceTypeConvention { get; set; }
        public ILinkNameConvention LinkNameConvention { get; set; }
        public ILinkIdConvention LinkIdConvention { get; set; }
        public IPropertyScanningConvention PropertyScanningConvention { get; set; }

        public ResourceConfigurationBuilder(ConfigurationBuilder configurationBuilder)
        {
            ResourceTypeConvention = configurationBuilder.GetConvention<IResourceTypeConvention>();
            LinkNameConvention = configurationBuilder.GetConvention<ILinkNameConvention>();
            LinkIdConvention = configurationBuilder.GetConvention<ILinkIdConvention>();
            PropertyScanningConvention = configurationBuilder.GetConvention<IPropertyScanningConvention>();

            BuiltResourceMapping = new ResourceMapping<TResource>
            {
                ResourceType = ResourceTypeConvention.GetResourceTypeFromRepresentationType(typeof(TResource))
            };
        }

        public ResourceConfigurationBuilder<TResource, TController> WithResourceType(string resourceType)
        {
            BuiltResourceMapping.ResourceType = resourceType;
            return this;
        }

        public ResourceConfigurationBuilder<TResource, TController> WithIdSelector(Expression<Func<TResource, object>> expression)
        {
            BuiltResourceMapping.IdGetter = ExpressionUtils.CompileToObjectTypedFunction(expression);
            BuiltResourceMapping.IdSetter = CreateIdSetter(expression.GetPropertyInfo());
            return this;
        }

        public ResourceConfigurationBuilder<TResource, TController> WithSimpleProperty(Expression<Func<TResource, object>> propertyAccessor)
        {
            var propertyInfo = propertyAccessor.GetPropertyInfo();
            AddProperty(propertyInfo, typeof(TResource));
            return this;
        }

        public ResourceConfigurationBuilder<TResource, TController> WithSimpleProperty(Expression<Func<TResource, object>> propertyAccessor, SerializationDirection direction)
        {
            var propertyInfo = propertyAccessor.GetPropertyInfo();
            RemoveProperty(propertyInfo);
            AddProperty(propertyInfo, typeof(TResource), direction);
            return this;
        }

        public ResourceConfigurationBuilder<TResource, TController> IgnoreProperty(Expression<Func<TResource, object>> propertyAccessor)
        {
            var pi = propertyAccessor.GetPropertyInfo();
            RemoveProperty(pi);
            return this;
        }

        public ResourceConfigurationBuilder<TResource, TController> WithLinkTemplate(string link)
        {
            BuiltResourceMapping.UrlTemplate = link;
            return this;
        }

        /// <summary>
        /// Registers through discovery all properties that are considered primitive, i.e. not linked resources.
        /// </summary>
        /// <remarks>
        /// Conventions used:
        /// IProperyScanningConvention
        /// 
        /// Supply a custom implementations to alter behavior.
        /// </remarks>
        public ResourceConfigurationBuilder<TResource, TController> WithAllSimpleProperties()
        {
            foreach (var propertyInfo in typeof(TResource).GetProperties())
            {
                if (PropertyScanningConvention.IsPrimaryId(propertyInfo))
                {
                    BuiltResourceMapping.IdGetter = propertyInfo.GetValue;
                    PropertyInfo info = propertyInfo;
                    BuiltResourceMapping.IdSetter = CreateIdSetter(info);
                }
                else if (!PropertyScanningConvention.IsLinkedResource(propertyInfo) && !PropertyScanningConvention.ShouldIgnore(propertyInfo))
                    AddProperty(propertyInfo, typeof(TResource));
            }
            return this;
        }

        public ResourceConfigurationBuilder<TResource, TController> WithLinkedChildResources()
        {
            return this;
        }

        private static Action<object, string> CreateIdSetter(PropertyInfo info)
        {
            return (o, s) => info.SetValue(o, ParseStringIntoT(info.PropertyType, s));
        }

        public static object ParseStringIntoT(Type t, string s)
        {
            var foo = TypeDescriptor.GetConverter(t);
            return foo.ConvertFromInvariantString(s);
        }

        /// <summary>
        /// Registers through discovery all properties, including primitive properties and linked resources.
        /// </summary>
        /// <remarks>
        /// Conventions used:
        /// IResourceTypeConvention
        /// ILinkIdConvention
        /// ILinkNameConvention
        /// IProperyScanningConvention
        /// 
        /// Supply a custom implementations to alter behavior.
        /// </remarks>
        public ResourceConfigurationBuilder<TResource, TController> WithAllProperties()
        {
            WithAllSimpleProperties();
            WithAllLinkedResources();
            return this;
        }

        /// <summary>
        /// Registers all properties discovered to be linked resources.
        /// </summary>
        /// <remarks>
        /// Conventions used:
        /// IResourceTypeConvention
        /// ILinkIdConvention
        /// ILinkNameConvention
        /// IProperyScanningConvention
        /// 
        /// Supply a custom implementations to alter behavior.
        /// </remarks>
        public ResourceConfigurationBuilder<TResource, TController> WithAllLinkedResources()
        {
            MethodInfo openMethod = GetType().GetMethod("WithLinkedResource");

            foreach (var propertyInfo in typeof(TResource).GetProperties())
            {
                if (PropertyScanningConvention.IsLinkedResource(propertyInfo) &&
                    !PropertyScanningConvention.ShouldIgnore(propertyInfo))
                {
                    var nestedType = propertyInfo.PropertyType;
                    var parameterExp = Expression.Parameter(typeof(TResource));
                    var propertyExp = Expression.Property(parameterExp, propertyInfo);
                    var propertyAccessor = Expression.Lambda(propertyExp, parameterExp);

                    // Because the expression is constructed in run-time and we need to invoke the convention that expects
                    // a compile-time-safe generic method, reflection must be used to invoke it.
                    MethodInfo closedMethod = openMethod.MakeGenericMethod(nestedType);
                    closedMethod.Invoke(this, new object[] { propertyAccessor, null, null, null, ResourceInclusionRules.Smart, null, null });
                }
            }
            return this;
        }

        private Type GetItemType(Type ienumerableType)
        {
            return ienumerableType
                .GetInterfaces()
                .Where(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetGenericArguments()[0])
                .SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Conventions used:
        /// IResourceTypeConvention
        /// ILinkIdConvention
        /// ILinkNameConvention
        /// 
        /// Supply a custom implementations to alter behavior.
        /// </remarks>
        public ResourceConfigurationBuilder<TResource, TController> WithLinkedResource<TNested>(
            Expression<Func<TResource, TNested>> objectAccessor, 
            Expression<Func<TResource, object>> idAccessor = null, 
            string linkedResourceType = null, 
            string linkName = null, 
            ResourceInclusionRules inclusionRule = ResourceInclusionRules.Smart, 
            string relatedURLTemplate = null, 
            string selfURLTemplate = null) 
            where TNested : class
        {
            if (typeof(TNested).Name == "Array")
                throw new NotSupportedException("Array type is not supported!");

            var propertyInfo = objectAccessor.GetPropertyInfo();

            var isCollection = typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);

            var linkedType = isCollection ? GetItemType(typeof(TNested)) : typeof(TNested);

            if (linkName == null) linkName = LinkNameConvention.GetLinkNameFromExpression(objectAccessor);
            if (linkedResourceType == null) linkedResourceType = ResourceTypeConvention.GetResourceTypeFromRepresentationType(linkedType);
            if (idAccessor == null) idAccessor = LinkIdConvention.GetIdExpression(objectAccessor);

            var link = new RelationshipMapping<TResource, TNested>
            {
                RelationshipName = linkName,
                ResourceIdGetter = idAccessor,
                ResourceGetter = ExpressionUtils.CompileToObjectTypedExpression(objectAccessor),
                IsCollection = isCollection,
                RelatedCollectionProperty = isCollection ? new PropertyHandle<TResource, TNested>(objectAccessor) : null,
                RelatedBaseType = linkedType,
                RelatedBaseResourceType = linkedResourceType,
                InclusionRule = inclusionRule
            };

            BuiltResourceMapping.Relationships.Add(link);
            return this;
        }

        private void AddProperty(PropertyInfo propertyInfo, Type type, SerializationDirection direction = SerializationDirection.Both)
        {
            var name = PropertyScanningConvention.GetPropertyName(propertyInfo);
            if (BuiltResourceMapping.PropertyGetters.ContainsKey(name) ||
                BuiltResourceMapping.PropertySetters.ContainsKey(name))
            {
                throw new InvalidOperationException(string.Format("Property {0} is already registered on type {1}.", name, typeof(TResource)));
            }

            if (direction == SerializationDirection.Out || direction == SerializationDirection.Both)
            {
                BuiltResourceMapping.PropertyGetters[name] = propertyInfo.GetValue;
            }

            if (direction == SerializationDirection.In || direction == SerializationDirection.Both)
            {
                BuiltResourceMapping.PropertySetters[name] = propertyInfo.SetValue;
            }
            
            var instance = Expression.Parameter(typeof(object), "i");
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(
                Expression.Convert(instance, propertyInfo.DeclaringType),
                propertyInfo.GetSetMethod(),
                Expression.Convert(argument, propertyInfo.PropertyType));

            Expression<Action<object, object>> expression = Expression.Lambda<Action<object, object>>(setterCall, instance, argument);

            if (direction == SerializationDirection.In || direction == SerializationDirection.Both)
            {
                BuiltResourceMapping.PropertySettersExpressions[name] = expression;
            }
        }

        private void RemoveProperty(PropertyInfo propertyInfo)
        {
            var name = PropertyScanningConvention.GetPropertyName(propertyInfo);
            BuiltResourceMapping.PropertyGetters.Remove(name);
            BuiltResourceMapping.PropertySetters.Remove(name);
            BuiltResourceMapping.PropertySettersExpressions.Remove(name);
        }
    }
}
