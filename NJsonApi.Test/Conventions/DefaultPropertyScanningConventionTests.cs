using FluentAssertions;
using NUnit.Framework;
using UtilJsonApiSerializer.Conventions.Impl;
using UtilJsonApiSerializer.Test.TestModel;

namespace UtilJsonApiSerializer.Test.Conventions
{
    public class DefaultPropertyScanningConventionTests
    {
        [Theory]
        public void Distinguishes_linked_resources()
        {
            // Arrange
            var convention = new DefaultPropertyScanningConvention();
            var titlePi = typeof(Post).GetProperty("Title");
            var authorPi = typeof(Post).GetProperty("Author");
            var repliesPi = typeof(Post).GetProperty("Replies");

            // Act
            var titleIsLinkedResource = convention.IsLinkedResource(titlePi);
            var authorIsLinkedResource = convention.IsLinkedResource(authorPi);
            var repliesIsLinkedResource = convention.IsLinkedResource(repliesPi);

            // Assert
            titleIsLinkedResource.Should().BeFalse();
            authorIsLinkedResource.Should().BeTrue();
            repliesIsLinkedResource.Should().BeTrue();
        }

        [Theory]
        public void Distinguishes_primary_id()
        {
            // Arrange
            var convention = new DefaultPropertyScanningConvention();
            var titlePi = typeof(Post).GetProperty("Title");
            var authorIdPi = typeof(Post).GetProperty("AuthorId");
            var idPi = typeof(Post).GetProperty("Id");

            // Act
            var titleIsPrimaryId = convention.IsPrimaryId(titlePi);
            var authorIdPiIsPrimaryId = convention.IsPrimaryId(authorIdPi);
            var idIsPrimaryId = convention.IsPrimaryId(idPi);
            

            // Assert
            titleIsPrimaryId.Should().BeFalse();
            authorIdPiIsPrimaryId.Should().BeFalse();
            idIsPrimaryId.Should().BeTrue();
        }

        [Theory]
        public void Distinguishes_ignored_properties()
        {
            // Arrange
            var convention = new DefaultPropertyScanningConvention();
            var titlePi = typeof(Post).GetProperty("Title");
            var internalNumberPi = typeof(Post).GetProperty("InternalNumber");

            // Act
            var shouldIgnoreTitle = convention.ShouldIgnore(titlePi);
            var shouldIgnoreInternalNumber = convention.ShouldIgnore(internalNumberPi);

            // Assert
            shouldIgnoreTitle.Should().BeFalse();
            shouldIgnoreInternalNumber.Should().BeTrue();
        }

        [Theory]
        public void Converts_property_name_to_camel_case()
        {
            // Arrange
            var convention = new DefaultPropertyScanningConvention();
            var titlePi = typeof(Post).GetProperty("Title");

            // Act
            var name = convention.GetPropertyName(titlePi);

            // Assert
            name.Should().Be("title");
        }
    }
}
