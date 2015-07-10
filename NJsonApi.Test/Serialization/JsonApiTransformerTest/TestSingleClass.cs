using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialCee.Framework.NJsonApi.Serialization;
using SoftwareApproach.TestingExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialCee.Framework.NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    [TestClass]
    public class TestSingleClass
    {
        readonly List<string> reservedKeys = new List<string> { "id", "type", "href", "links" };
           
        [TestMethod]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            result.Data.ShouldHaveCountOf(1);
            var transformedObject = result.Data["sampleClasses"] as Dictionary<string, object>;
            transformedObject.ShouldNotBeNull();
        }

        [TestMethod]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as Dictionary<string, object>;
            transformedObject["id"].ShouldEqual(objectToTransform.Id.ToString());
        }

        [TestMethod]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as Dictionary<string, object>;
            transformedObject["someValue"].ShouldEqual(objectToTransform.SomeValue);
            transformedObject["date"].ShouldEqual(objectToTransform.DateTime);
            transformedObject.Keys.Where(k => !reservedKeys.Contains(k)).ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_href()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as Dictionary<string, object>;
            transformedObject["href"].ShouldEqual("http://sampleclass/1");
        }

        [TestMethod]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as Dictionary<string, object>;
            transformedObject["type"].ShouldEqual("sampleClasses");
        }

        private static SampleClass CreateObjectToTransform()
        {
            return new SampleClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped"
            };
        }

        private Configuration CreateConfiguration()
        {
            var conf = new Configuration();
            var mapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime );
            conf.AddMapping(mapping);

            return conf;

        }

        class SampleClass
        {
            public int Id { get; set; }
            public string SomeValue { get; set; }
            public DateTime DateTime { get; set; }
            public string NotMappedValue { get; set; }
        }
    }
}