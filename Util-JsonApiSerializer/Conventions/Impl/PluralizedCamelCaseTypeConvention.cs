using System;
#if NETCOREAPP
using Pluralize;
#else
using System.Data.Entity.Design.PluralizationServices;
#endif
using System.Globalization;
using UtilJsonApiSerializer.Utils;

namespace UtilJsonApiSerializer.Conventions.Impl
{
    public class PluralizedCamelCaseTypeConvention : IResourceTypeConvention
    {
#if NETCOREAPP
        protected Pluralize.NET.Pluralizer PluralizationService { get; private set; }
#else
        protected PluralizationService PluralizationService { get; private set; }
#endif
        public PluralizedCamelCaseTypeConvention()
        {
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
#if NETCOREAPP
            PluralizationService = new Pluralize.NET.Pluralizer();
            #else
            PluralizationService = PluralizationService.CreateService(cultureInfo);
            #endif
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
