using NJsonApi.Test.TestModel;
using NJsonApi.Conventions.Impl;
using Xunit;

namespace NJsonApi.Test.Conventions
{
    public class CamelCaseLinkNameConventionTests
    {
        [Fact]
        public void Converts_collection_links()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Author a) => a.Posts);

            // Assert
            Assert.Equal(name, "posts");
        }

        [Fact]
        public void Converts_single_links()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Post a) => a.Author);

            // Assert
            Assert.Equal(name, "author");
        }

        [Fact]
        public void Converts_distinct_collection_names()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Post a) => a.Replies);

            // Assert
            Assert.Equal(name, "replies");
        }
    }
}
