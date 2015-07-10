using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialCee.Framework.NJsonApi.Conventions.Impl;
using SocialCee.Framework.NJsonApi.Test.TestModel;
using SoftwareApproach.TestingExtensions;

namespace SocialCee.Framework.NJsonApi.Test.Conventions
{
    [TestClass]
    public class CamelCaseLinkNameConventionTests
    {
        [TestMethod]
        public void Converts_collection_links()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Author a) => a.Posts);

            // Assert
            name.ShouldEqual("posts");
        }

        [TestMethod]
        public void Converts_single_links()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Post a) => a.Author);

            // Assert
            name.ShouldEqual("author");
        }

        [TestMethod]
        public void Converts_distinct_collection_names()
        {
            // Arrange
            var convention = new CamelCaseLinkNameConvention();

            // Act
            var name = convention.GetLinkNameFromExpression((Post a) => a.Replies);

            // Assert
            name.ShouldEqual("replies");
        }
    }
}
