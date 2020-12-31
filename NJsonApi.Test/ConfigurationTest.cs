using System;
using FluentAssertions;
using NUnit.Framework;

namespace UtilJsonApiSerializer.Test
{
    public class ConfigurationTest
    {
        class SampleClass
        {
            public int Id { get; set; }
            public string Value { get; set; }

            public int NestedObjectId { get; set; }
            public NestedClass NestedClass { get; set; }
        }

        class NestedClass
        {
            public int Id { get; set; }
            public string OtherValue { get; set; }
        }

        [Theory]
        public void Creates_configuration_mapping()
        {
            // Arrange
            var sampleMapping = new ResourceMapping<SampleClass>(c => c.Id, "sample_{id}")
            {
                ResourceType = "sampleClasses"
            };

            sampleMapping.AddPropertyGetter("value", c => c.Value);

            var conf = new Configuration();

            // Act
            conf.AddMapping(sampleMapping);

            // Assert
            conf.IsMappingRegistered(typeof(SampleClass)).Should().BeTrue();
            conf.GetMapping(typeof(SampleClass)).Should().NotBeNull();
            conf.IsMappingRegistered(typeof(NestedClass)).Should().BeFalse();
            conf.GetMapping(typeof(NestedClass)).Should().BeNull();
        }
    }
}