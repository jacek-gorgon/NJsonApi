using Xunit;

namespace NJsonApi.Test.Configuration
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

        [Fact]
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
            Assert.True(conf.IsMappingRegistered(typeof(SampleClass)));
            Assert.NotNull(conf.GetMapping(typeof(SampleClass)));
            Assert.False(conf.IsMappingRegistered(typeof(NestedClass)));
            Assert.Null(conf.GetMapping(typeof(NestedClass)));
        }
    }
}