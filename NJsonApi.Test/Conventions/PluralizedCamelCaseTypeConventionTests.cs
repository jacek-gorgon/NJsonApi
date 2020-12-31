using FluentAssertions;
using NUnit.Framework;
using UtilJsonApiSerializer.Conventions.Impl;
using UtilJsonApiSerializer.Test.TestModel;

namespace UtilJsonApiSerializer.Test.Conventions
{
    public class PluralizedCamelCaseTypeConventionTests
    {
        [Theory]
        public void Supports_english_singular_single_word_type_name()
        {
            // Arrange
            var convention = new PluralizedCamelCaseTypeConvention();

            // Act
            var name = convention.GetResourceTypeFromRepresentationType(typeof(Author));

            // Assert
            name.Should().Be("authors");
        }

        [Theory]
        public void Supports_english_singular_pascal_case_type_name()
        {
            // Arrange
            var convention = new PluralizedCamelCaseTypeConvention();

            // Act
            var name = convention.GetResourceTypeFromRepresentationType(typeof(PostLike));

            // Assert
            name.Should().Be("postLikes");
        }
    }
}
