using System.Collections.Generic;
using NJsonApi.Infrastructure;
using Xunit;

namespace NJsonApi.Test.Infrastructure
{
    public class MetadataWrapperTest
    {
        [Fact]
        public void MetadataWrapper_using_ctor_string_ok()
        {
            // Arrange
            const string testString = "Test String";
            
            // Act
            var sut = new MetaDataWrapper<string>(testString);

            // Assert
            Assert.Equal(sut.Value, testString);
            Assert.Empty(sut.MetaData);
        }

        [Fact]
        public void MetadataWrapper_add_result_collection_ok()
        {
            // Arrange
            var testsStrings = new List<string>(){ "test1", "test2" };

            // Act
            var sut = new MetaDataWrapper<List<string>>(testsStrings);

            // Assert
            Assert.Equal(sut.Value, testsStrings);
            Assert.Empty(sut.MetaData);
        }
    }
}