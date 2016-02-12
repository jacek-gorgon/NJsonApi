using NJsonApi.Test.TestModel;
using NJsonApi.Conventions.Impl;
using Xunit;

namespace NJsonApi.Test.Conventions
{
    public class SimpleLinkedIdConventionTests
    {
        [Fact]
        public void For_collection_links_gracefully_fails()
        {
            // Arrange
            var convention = new SimpleLinkedIdConvention();

            // Act
            var expression = convention.GetIdExpression((Author a) => a.Posts);

            // Assert
            Assert.Null(expression);
        }

        [Fact]
        public void For_absent_id_properties_gracefully_fails()
        {
            // Arrange
            var convention = new SimpleLinkedIdConvention();

            // Act
            var expression = convention.GetIdExpression((Comment c) => c.Post);

            // Assert
            Assert.Null(expression);
        }

        [Fact]
        public void For_existing_id_properties_returns_proper_expression()
        {
            // Arrange
            var convention = new SimpleLinkedIdConvention();
            var post = new Post {AuthorId = 4};

            // Act
            var expression = convention.GetIdExpression((Post p) => p.Author);

            // Assert
            Assert.NotNull(expression);
            var compiled = expression.Compile();
            Assert.Equal(compiled(post), 4);
        }
    }
}
