using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using NJsonApi.Utils;

namespace NJsonApi.Conventions.Impl
{
    internal class DefaultPropertyScanningConvention : IPropertyScanningConvention
    {
        private List<string> reservedPropertyKeys = new List<string>()
        {
            "id",
            "href",
            "type",
            "links"
        };

        public DefaultPropertyScanningConvention()
        {
            ThrowOnUnmappedLinkedType = true;
        }

        /// <summary>
        /// Determines if the given PropertyInfo is the primary ID of the currently scanned resource.
        /// </summary>
        public virtual bool IsPrimaryId(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name == "Id";
        }

        /// <summary>
        /// Used to distinguish simple properties (serialized in-line) from linked resources (side-loaded in "linked" section).
        /// </summary>
        public virtual bool IsLinkedResource(PropertyInfo pi)
        {
            var type = pi.PropertyType;
            bool isPrimitiveType = type.GetTypeInfo().IsPrimitive || type.GetTypeInfo().IsValueType || (type == typeof(string) || (type == typeof(DateTime)) || (type == typeof(TimeSpan)) || (type == typeof(DateTimeOffset)));
            return !isPrimitiveType;
        }

        /// <summary>
        /// Determines if the property should be ignored during scanning.
        /// </summary>
        public virtual bool ShouldIgnore(PropertyInfo pi)
        {
            return pi.GetCustomAttribute<JsonIgnoreAttribute>() != null;
        }

        /// <summary>
        /// If set to true, any scanned property that is discovered to be a linked resource, but is never registered in the builder, 
        /// will cause an exception to be thrown during build time.
        /// 
        /// If set to false, scanned properties that are discovered to be linked resources are silently removed from the mapping during build
        /// and ignored.
        /// </summary>
        public bool ThrowOnUnmappedLinkedType { get; set; }

        /// <summary>
        /// Gets the name of the property as it gets serialized in JSON.
        /// </summary>
        public string GetPropertyName(PropertyInfo pi)
        {
            var name = CamelCaseUtil.ToCamelCase(pi.Name);
            if (reservedPropertyKeys.Contains(name.ToLower()))
            {
                name = string.Format("_{0}", name.ToLower());
            }
            return name;
        }
    }
}
