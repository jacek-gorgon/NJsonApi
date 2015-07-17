using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using NJsonApi.Utils;

namespace NJsonApi.Conventions.Impl
{
    public class PluralizedCamelCaseTypeConvention : IResourceTypeConvention
    {
        protected PluralizationService PluralizationService { get; private set; }
        public PluralizedCamelCaseTypeConvention()
        {
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            PluralizationService = PluralizationService.CreateService(cultureInfo);
        }

        public virtual string GetResourceTypeFromRepresentationType(Type resourceType)
        {
            string name = resourceType.Name;
            name = Pluralize(name);
            name = Camelize(name);
            return name;
        }

        protected virtual string Pluralize(string name)
        {
            return PluralizationService.IsSingular(name) ? PluralizationService.Pluralize(name) : name;
        }

        protected virtual string Camelize(string name)
        {
            return CamelCaseUtil.ToCamelCase(name);
        }
    }
}
