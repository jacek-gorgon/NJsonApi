using System;
using Humanizer;
using System.Globalization;
using NJsonApi.Utils;

namespace NJsonApi.Conventions.Impl
{
    public class PluralizedCamelCaseTypeConvention : IResourceTypeConvention
    {
        public virtual string GetResourceTypeFromRepresentationType(Type resourceType)
        {
            string name = resourceType.Name;
            name = Pluralize(name);
            name = Camelize(name);
            return name;
        }

        protected virtual string Pluralize(string name) => name.Pluralize(inputIsKnownToBeSingular: false);
        protected virtual string Camelize(string name) => name.Camelize();
    }
}
