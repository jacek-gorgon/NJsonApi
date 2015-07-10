using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Common.Infrastructure;
using SoftwareApproach.TestingExtensions;

namespace SocialCee.Framework.Common.Test.Infrastructure
{
    [TestClass]
    public class MetadataWrapperTest
    {
        [TestMethod]
        public void MetadataWrapper_using_ctor_string_ok()
        {
            // Arrange
            const string testString = "Test String";
            
            // Act
            var sut = new MetaDataWrapper<string>(testString);

            // Assert
            sut.Value.ShouldEqual(testString);
            sut.MetaData.ShouldBeEmpty();
        }

        [TestMethod]
        public void MetadataWrapper_add_result_collection_ok()
        {
            // Arrange
            var testsStrings = new List<string>(){ "test1", "test2" };

            // Act
            var sut = new MetaDataWrapper<List<string>>(testsStrings);

            // Assert
            sut.MetaData.ShouldBeEmpty();
            sut.Value.ShouldEqual(testsStrings);
        }
    }
}