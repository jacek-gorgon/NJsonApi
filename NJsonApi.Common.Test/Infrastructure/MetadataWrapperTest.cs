using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UtilJsonApiSerializer.Common.Infrastructure;

namespace UtilJsonApiSerializer.Common.Test.Infrastructure
{
    public class MetadataWrapperTest
    {
        [Theory]
        public void MetadataWrapper_using_ctor_string_ok()
        {
            // Arrange
            const string testString = "Test String";
            
            // Act
            var sut = new MetaDataWrapper<string>(testString);

            // Assert
            sut.Value.Should().Be(testString);
            sut.MetaData.Should().BeEmpty();
        }

        [Theory]
        public void MetadataWrapper_add_result_collection_ok()
        {
            // Arrange
            var testsStrings = new List<string>(){ "test1", "test2" };

            // Act
            var sut = new MetaDataWrapper<List<string>>(testsStrings);

            // Assert
            sut.MetaData.Should().BeEmpty();
            sut.Value.Should().BeEquivalentTo(testsStrings);
        }
    }
}