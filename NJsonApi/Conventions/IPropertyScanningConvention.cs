using System.Reflection;

namespace NJsonApi.Conventions
{
    /// <summary>
    /// Represents a set of conventions for property scanning.
    /// </summary>
    public interface IPropertyScanningConvention : IConvention
    {
        /// <summary>
        /// Determines if the given PropertyInfo is the primary ID of the currently scanned resource.
        /// </summary>
        bool IsPrimaryId(PropertyInfo propertyInfo);

        /// <summary>
        /// Used to distinguish simple properties (serialized in-line) from linked resources (side-loaded in "linked" section).
        /// </summary>
        bool IsLinkedResource(PropertyInfo pi);

        /// <summary>
        /// Determines if the property should be ignored during scanning.
        /// </summary>
        bool ShouldIgnore(PropertyInfo pi);

        /// <summary>
        /// If set to true, any scanned property that is discovered to be a linked resource, but is never registered in the builder, 
        /// will cause an exception to be thrown during build time.
        /// If set to false, scanned properties that are discovered to be linked resources are silently removed from the mapping during build
        /// and ignored.
        /// </summary>
        bool ThrowOnUnmappedLinkedType { get; }

        /// <summary>
        /// Gets the name of the property as it gets serialized in JSON.
        /// </summary>
        string GetPropertyName(PropertyInfo pi);
    }
}
