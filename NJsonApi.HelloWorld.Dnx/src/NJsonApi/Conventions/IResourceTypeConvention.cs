using System;

namespace NJsonApi.Conventions
{
    /// <summary>
    /// Represents a convention for calculating resource types (JSON.API term) based on the resource representation type (actual class representing the resource).
    /// </summary>
    public interface IResourceTypeConvention : IConvention
    {
        string GetResourceTypeFromRepresentationType(Type resourceRepresentationType);
    }
}