using NJsonApi.Test.TestControllers;
using NJsonApi.Test.TestModel;
using Xunit;

namespace NJsonApi.Test.Configuration
{
    public class ConfigurationTest
    {
        [Fact]
        public void Creates_configuration_mapping()
        {
            // Arrange
            var sampleMapping = new ResourceMapping<Post, PostsController>(c => c.Id, "sample_{id}")
            {
                ResourceType = "posts"
            };

            sampleMapping.AddPropertyGetter("value", c => c.Title);

            var conf = new NJsonApi.Configuration();

            // Act
            conf.AddMapping(sampleMapping);

            // Assert
            Assert.True(conf.IsMappingRegistered(typeof(Post)));
            Assert.NotNull(conf.GetMapping(typeof(Post)));
            Assert.False(conf.IsMappingRegistered(typeof(Author)));
            Assert.Null(conf.GetMapping(typeof(Author)));
        }
    }
}