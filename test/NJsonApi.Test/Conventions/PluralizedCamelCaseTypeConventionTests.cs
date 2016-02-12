using NJsonApi.Test.TestModel;
using NJsonApi.Conventions.Impl;
using Xunit;

namespace NJsonApi.Test.Conventions
{
    public class PluralizedCamelCaseTypeConventionTests
    {
        [Fact]
        public void Supports_english_singular_single_word_type_name()
        {
            // Arrange
            var convention = new PluralizedCamelCaseTypeConvention();

            // Act
            var name = convention.GetResourceTypeFromRepresentationType(typeof(Author));

            // Assert
            Assert.Equal(name, "authors");
        }

        [Fact]
        public void Supports_english_singular_pascal_case_type_name()
        {
            // Arrange
            var convention = new PluralizedCamelCaseTypeConvention();

            // Act
            var name = convention.GetResourceTypeFromRepresentationType(typeof(PostLike));

            // Assert
            Assert.Equal(name, "postLikes");
        }
    }
}
