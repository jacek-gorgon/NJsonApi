using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;

namespace SocialCee.Framework.NJsonApi.Test
{
    [TestClass]
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

        [TestMethod]
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
            conf.IsMappingRegistered(typeof(SampleClass)).ShouldBeTrue();
            conf.GetMapping(typeof(SampleClass)).ShouldNotBeNull();
            conf.IsMappingRegistered(typeof(NestedClass)).ShouldBeFalse();
            conf.GetMapping(typeof(NestedClass)).ShouldBeNull();
        }
    }
}