using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Test.TestModel;
using NJsonApi.Conventions.Impl;
using SoftwareApproach.TestingExtensions;

namespace NJsonApi.Test.Conventions
{
    [TestClass]
    public class SimpleLinkedIdConventionTests
    {
        [TestMethod]
        public void For_collection_links_gracefully_fails()
        {
            // Arrange
            var convention = new SimpleLinkedIdConvention();

            // Act
            var expression = convention.GetIdExpression((Author a) => a.Posts);

            // Assert
            expression.ShouldBeNull();
        }

        [TestMethod]
        public void For_absent_id_properties_gracefully_fails()
        {
            // Arrange
            var convention = new SimpleLinkedIdConvention();

            // Act
            var expression = convention.GetIdExpression((Comment c) => c.Post);

            // Assert
            expression.ShouldBeNull();
        }

        [TestMethod]
        public void For_existing_id_properties_returns_proper_expression()
        {
            // Arrange
            var convention = new SimpleLinkedIdConvention();
            var post = new Post {AuthorId = 4};

            // Act
            var expression = convention.GetIdExpression((Post p) => p.Author);

            // Assert
            expression.ShouldNotBeNull();
            var compiled = expression.Compile();
            compiled(post).ShouldEqual(4);
        }
    }
}
