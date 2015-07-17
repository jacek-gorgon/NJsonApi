using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NJsonApi.Conventions;
using NJsonApi.Utils;

namespace NJsonApi
{
    public class ResourceConfigurationBuilder<TResource> : IResourceConfigurationBuilder
    {
        public IResourceMapping ConstructedMetadata { get; set; }
        public ConfigurationBuilder ConfigurationBuilder { get; set; }

        public IResourceTypeConvention ResourceTypeConvention { get; set; }
        public ILinkNameConvention LinkNameConvention { get; set; }
        public ILinkIdConvention LinkIdConvention { get; set; }
        public IPropertyScanningConvention PropertyScanningConvention { get; set; }

        public ResourceConfigurationBuilder(ConfigurationBuilder configurationBuilder)
        {
            ConfigurationBuilder = configurationBuilder;

            ResourceTypeConvention = configurationBuilder.GetConvention<IResourceTypeConvention>();
            LinkNameConvention = configurationBuilder.GetConvention<ILinkNameConvention>();
            LinkIdConvention = configurationBuilder.GetConvention<ILinkIdConvention>();
            PropertyScanningConvention = configurationBuilder.GetConvention<IPropertyScanningConvention>();

            ConstructedMetadata = new ResourceMapping<TResource>
            {
                ResourceType = ResourceTypeConvention.GetResourceTypeFromRepresentationType(typeof(TResource))
            };
        }

        public ResourceConfigurationBuilder<TResource> WithResourceType(string resourceType)
        {
            ConstructedMetadata.ResourceType = resourceType;
            return this;
        }

        public ResourceConfigurationBuilder<TResource> WithIdSelector(Expression<Func<TResource, object>> expression)
        {
            ConstructedMetadata.IdGetter = ExpressionUtils.CompileToObjectTypedFunction(expression);
            ConstructedMetadata.IdSetter = CreateIdSetter(ExpressionUtils.GetPropertyInfoFromExpression(expression));
            return this;
        }

        public ResourceConfigurationBuilder<TResource> WithSimpleProperty(Expression<Func<TResource, object>> propertyAccessor)
        {
            var propertyInfo = ExpressionUtils.GetPropertyInfoFromExpression(propertyAccessor);
            AddProperty(propertyInfo, typeof(TResource));
            return this;
        }

        public ResourceConfigurationBuilder<TResource> WithSimpleProperty(Expression<Func<TResource, object>> propertyAccessor, SerializationDirection direction)
        {
            var propertyInfo = ExpressionUtils.GetPropertyInfoFromExpression(propertyAccessor);
            RemoveProperty(propertyInfo);
            AddProperty(propertyInfo, typeof(TResource), direction);
            return this;
        }

        public ResourceConfigurationBuilder<TResource> IgnoreProperty(Expression<Func<TResource, object>> propertyAccessor)
        {
            var pi = ExpressionUtils.GetPropertyInfoFromExpression(propertyAccessor);
            RemoveProperty(pi);
            return this;
        }


        public ResourceConfigurationBuilder<TResource> WithLinkTemplate(string link)
        {
            ConstructedMetadata.UrlTemplate = link;
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
        public ResourceConfigurationBuilder<TResource> WithAllSimpleProperties()
        {
            foreach (var propertyInfo in typeof(TResource).GetProperties())
            {
                if (PropertyScanningConvention.IsPrimaryId(propertyInfo))
                {
                    ConstructedMetadata.IdGetter = propertyInfo.GetValue;
                    PropertyInfo info = propertyInfo;
                    ConstructedMetadata.IdSetter = CreateIdSetter(info);
                }
                else if (!PropertyScanningConvention.IsLinkedResource(propertyInfo) && !PropertyScanningConvention.ShouldIgnore(propertyInfo))
                    AddProperty(propertyInfo, typeof(TResource));
            }
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
        public ResourceConfigurationBuilder<TResource> WithAllProperties()
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
        public ResourceConfigurationBuilder<TResource> WithAllLinkedResources()
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
                    closedMethod.Invoke(this, new object[] { propertyAccessor, null, null, null, null });
                }
            }
            return this;
        }

        private Type GetItemType(Type ienumerableType)
        {
            return ienumerableType
                .GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
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
        public ResourceConfigurationBuilder<TResource> WithLinkedResource<TNested>(Expression<Func<TResource, TNested>> objectAccessor, Expression<Func<TResource, object>> idAccessor = null, string linkedResourceType = null, string linkName = null, bool serializeAsLinked = true) where TNested : class
        {
            if (typeof(TNested).Name == "Array")
                throw new NotSupportedException("Array type is not supported!");

            var propertyInfo = ExpressionUtils.GetPropertyInfoFromExpression(objectAccessor);

            var isCollection = typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);

            var linkedType = isCollection ? GetItemType(typeof(TNested)) : typeof(TNested);

            if (linkName == null) linkName = LinkNameConvention.GetLinkNameFromExpression(objectAccessor);
            if (linkedResourceType == null) linkedResourceType = ResourceTypeConvention.GetResourceTypeFromRepresentationType(linkedType);
            if (idAccessor == null) idAccessor = LinkIdConvention.GetIdExpression(objectAccessor);

            var link = new LinkMapping<TResource, TNested>
            {
                LinkName = linkName,
                ResourceIdGetter = idAccessor,
                ResourceGetter = ExpressionUtils.CompileToObjectTypedExpression(objectAccessor),
                IsCollection = isCollection,
                CollectionProperty = isCollection ? propertyInfo : null,
                LinkedType = linkedType,
                LinkedResourceType = linkedResourceType,
                SerializeAsLinked = serializeAsLinked
            };

            ConstructedMetadata.Links.Add(link);
            return this;
        }

        private void AddProperty(PropertyInfo propertyInfo, Type type, SerializationDirection direction = SerializationDirection.Both)
        {
            var name = PropertyScanningConvention.GetPropertyName(propertyInfo);
            if (ConstructedMetadata.PropertyGetters.ContainsKey(name) ||
                ConstructedMetadata.PropertySetters.ContainsKey(name))
            {
                throw new InvalidOperationException(string.Format("Property {0} is already registered on type {1}.", name, typeof(TResource)));
            }

            if (direction == SerializationDirection.Out || direction == SerializationDirection.Both)
            {
                ConstructedMetadata.PropertyGetters[name] = propertyInfo.GetValue;
            }

            if (direction == SerializationDirection.In || direction == SerializationDirection.Both)
            {
                ConstructedMetadata.PropertySetters[name] = propertyInfo.SetValue;    
            }
            
            var instance = Expression.Parameter(typeof(object), "i");
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(
                Expression.Convert(instance, propertyInfo.DeclaringType),
                propertyInfo.GetSetMethod(),
                Expression.Convert(argument, propertyInfo.PropertyType));

            Expression<Action<object, object>> expression = Expression.Lambda<Action<object, object>>(setterCall, instance, argument);

            if (direction == SerializationDirection.In  || direction == SerializationDirection.Both)
            {
                ConstructedMetadata.PropertySettersExpressions[name] = expression;
            }
        }

        private void RemoveProperty(PropertyInfo propertyInfo)
        {
            var name = PropertyScanningConvention.GetPropertyName(propertyInfo);
            ConstructedMetadata.PropertyGetters.Remove(name);
            ConstructedMetadata.PropertySetters.Remove(name);
            ConstructedMetadata.PropertySettersExpressions.Remove(name);
        }
    }
}
