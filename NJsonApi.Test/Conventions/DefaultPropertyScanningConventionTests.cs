using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Test.TestModel;
using NJsonApi.Conventions.Impl;
using SoftwareApproach.TestingExtensions;

namespace NJsonApi.Test.Conventions
{
    [TestClass]
    public class DefaultPropertyScanningConventionTests
    {
        [TestMethod]
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
            titleIsLinkedResource.ShouldBeFalse();
            authorIsLinkedResource.ShouldBeTrue();
            repliesIsLinkedResource.ShouldBeTrue();
        }

        [TestMethod]
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
            titleIsPrimaryId.ShouldBeFalse();
            authorIdPiIsPrimaryId.ShouldBeFalse();
            idIsPrimaryId.ShouldBeTrue();
        }

        [TestMethod]
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
            shouldIgnoreTitle.ShouldBeFalse();
            shouldIgnoreInternalNumber.ShouldBeTrue();
        }

        [TestMethod]
        public void Converts_property_name_to_camel_case()
        {
            // Arrange
            var convention = new DefaultPropertyScanningConvention();
            var titlePi = typeof(Post).GetProperty("Title");

            // Act
            var name = convention.GetPropertyName(titlePi);

            // Assert
            name.ShouldEqual("title");
        }
    }
}
