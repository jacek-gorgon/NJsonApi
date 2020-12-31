using FluentAssertions;
using NUnit.Framework;
using UtilJsonApiSerializer.Conventions.Impl;
using UtilJsonApiSerializer.Test.TestModel;

namespace UtilJsonApiSerializer.Test.Conventions
{
    public class CamelCaseLinkNameConventionTests
    {
        [Theory]
        public void Converts_collection_links()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Author a) => a.Posts);

            // Assert
            name.Should().Be("posts");
        }

        [Theory]
        public void Converts_single_links()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Post a) => a.Author);

            // Assert
            name.Should().Be("author");
        }

        [Theory]
        public void Converts_distinct_collection_names()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Post a) => a.Replies);

            // Assert
            name.Should().Be("replies");
        }
    }
}
