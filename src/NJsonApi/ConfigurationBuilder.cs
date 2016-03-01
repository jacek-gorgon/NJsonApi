using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NJsonApi.Conventions;
using NJsonApi.Conventions.Impl;
using NJsonApi.Utils;

namespace NJsonApi
{
    public class ConfigurationBuilder
    {
        public readonly Dictionary<Type, IResourceConfigurationBuilder> ResourceConfigurationsByType = new Dictionary<Type, IResourceConfigurationBuilder>();

        private readonly Stack<IConvention> conventions = new Stack<IConvention>();
        public ConfigurationBuilder()
        {
            //add the default conventions
            WithConvention(new PluralizedCamelCaseTypeConvention());
            WithConvention(new CamelCaseLinkNameConvention());
            WithConvention(new SimpleLinkedIdConvention());
            WithConvention(new DefaultPropertyScanningConvention());
        }

        public ConfigurationBuilder WithConvention<T>(T convention) where T : class, IConvention
        {
            conventions.Push(convention);
            return this;
        }

        public T GetConvention<T>() where T : class, IConvention
        {
            var firstMatchingConvention = conventions
                .OfType<T>()
                .FirstOrDefault();
            if (firstMatchingConvention == null)
                throw new InvalidOperationException(string.Format("No convention found for type {0}", typeof(T).Name));
            return firstMatchingConvention;
        }

        public ResourceConfigurationBuilder<TResource, TController> Resource<TResource, TController>()
        {
            var resource = typeof(TResource);

            if (DoesModelHaveReservedWordsRecursive(resource))
            {
                throw new InvalidOperationException("The model being registered for a resource contains properties that are reserved words by JsonApi.");
            }

            if (!ResourceConfigurationsByType.ContainsKey(resource))
            {
                var newResourceConfiguration = new ResourceConfigurationBuilder<TResource, TController>(this);
                ResourceConfigurationsByType[resource] = newResourceConfiguration;
                return newResourceConfiguration;
            }
            else
            {
                return ResourceConfigurationsByType[resource] as ResourceConfigurationBuilder<TResource, TController>;
            }
        }

        public Configuration Build()
        {
            var configuration = new Configuration();
            var propertyScanningConvention = GetConvention<IPropertyScanningConvention>();

            // Each link needs to be wired to full metadata once all resources are registered
            foreach (var resourceConfiguration in ResourceConfigurationsByType)
            {
                var links = resourceConfiguration.Value.BuiltResourceMapping.Relationships;
                for (int i = links.Count - 1; i >= 0; i--)
                {
                    var link = links[i];
                    IResourceConfigurationBuilder resourceConfigurationOutput;
                    if (!ResourceConfigurationsByType.TryGetValue(link.RelatedBaseType, out resourceConfigurationOutput))
                    {
                        if (propertyScanningConvention.ThrowOnUnmappedLinkedType)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    "Type {0} was registered to have a linked resource {1} of type {2} which was not registered. Register resource type {2} or disable serialization of that property.",
                                    link.ParentType.Name,
                                    link.RelationshipName,
                                    link.RelatedBaseType.Name));
                        }
                        else
                            links.RemoveAt(i);
                    }
                    else
                        link.ResourceMapping = resourceConfigurationOutput.BuiltResourceMapping;
                }

                configuration.AddMapping(resourceConfiguration.Value.BuiltResourceMapping);
            }

            return configuration;
        }

        private bool DoesModelHaveReservedWordsRecursive(Type model, List<Type> checkedTypes = null)
        {
            if (checkedTypes == null)
                checkedTypes = new List<Type>();

            if (checkedTypes.Contains(model))
            {
                return false;
            }
            else
            {
                checkedTypes.Add(model);
            }

            foreach (var property in model.GetProperties())
            {
                if (property.Name == "Relationships" || property.Name == "Links")
                {
                    return true;
                }

                var childTypesToScan = Reflection.FromWithinGeneric(property.PropertyType);

                foreach(var childType in childTypesToScan)
                {
                    if (childType.GetTypeInfo().IsClass)
                    {
                        return DoesModelHaveReservedWordsRecursive(childType, checkedTypes);
                    }
                }
            }
            return false;
        }
    }
}
