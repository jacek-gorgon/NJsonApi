﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using UtilJsonApiSerializer.Conventions.Impl;
using SoftwareApproach.TestingExtensions;
using UtilJsonApiSerializer.Test.TestModel;

namespace UtilJsonApiSerializer.Test.Conventions
{
    [TestClass]
    public class PluralizedCamelCaseTypeConventionTests
    {
        [TestMethod]
        public void Supports_english_singular_single_word_type_name()
        {
            // Arrange
            var convention = new PluralizedCamelCaseTypeConvention();

            // Act
            var name = convention.GetResourceTypeFromRepresentationType(typeof(Author));

            // Assert
            name.ShouldEqual("authors");
        }

        [TestMethod]
        public void Supports_english_singular_pascal_case_type_name()
        {
            // Arrange
            var convention = new PluralizedCamelCaseTypeConvention();

            // Act
            var name = convention.GetResourceTypeFromRepresentationType(typeof(PostLike));

            // Assert
            name.ShouldEqual("postLikes");
        }
    }
}
