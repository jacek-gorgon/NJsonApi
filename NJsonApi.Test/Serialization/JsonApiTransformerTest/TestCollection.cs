using System;
using System.Collections.Generic;
using System.Linq;
using MSTestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Common.Infrastructure;
using NJsonApi.Serialization;
using SoftwareApproach.TestingExtensions;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    [TestClass]
    public class TestCollection
    {
        readonly List<string> reservedKeys = new List<string> { "id", "type", "href", "links" };
           
        [TestMethod]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            result.Data.ShouldHaveCountOf(1);
            var transformedObject = result.Data["sampleClasses"] as List<Dictionary<string, object>>;
            transformedObject.ShouldNotBeNull();
        }

        [TestMethod]
        public void Creates_CompondDocument_for_collection_not_nested_single_class()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform().Take(1);
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            result.Data.ShouldHaveCountOf(1);
            var transformedObject = result.Data["sampleClasses"] as List<Dictionary<string, object>>;
            transformedObject.ShouldNotBeNull();
        }


        [TestMethod]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as List<Dictionary<string, object>>;
            transformedObject[0]["id"].ShouldEqual(objectsToTransform.First().Id.ToString());
            transformedObject[1]["id"].ShouldEqual(objectsToTransform.Last().Id.ToString());
        }

        [TestMethod]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as List<Dictionary<string, object>>;

            Action<Dictionary<string, object>, SampleClass> assertSame = (actual, expected) =>
            {
                actual["someValue"].ShouldEqual(expected.SomeValue);
                actual["date"].ShouldEqual(expected.DateTime);
                actual.Keys.Where(k => !reservedKeys.Contains(k)).ShouldHaveCountOf(2);
            };

            assertSame(transformedObject[0], objectsToTransform.First());
            assertSame(transformedObject[1], objectsToTransform.Last());
        }

        [TestMethod]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_href()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as List<Dictionary<string, object>>;
            transformedObject[0]["href"].ShouldEqual("http://sampleclass/1");
            transformedObject[1]["href"].ShouldEqual("http://sampleclass/2");
        }

        [TestMethod]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data["sampleClasses"] as List<Dictionary<string, object>>;
            transformedObject[0]["type"].ShouldEqual("sampleClasses");
            transformedObject[1]["type"].ShouldEqual("sampleClasses");
        }

        [TestMethod]
        public void Creates_CompondDocument_for_collectione_of_metadatawrapper_throws_notSupportedException()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            var objectsToTransform = new List<MetaDataWrapper<SampleClass>>
            {
                new MetaDataWrapper<SampleClass>(new SampleClass())
            };
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act => Assert
            ThrowsAssert.Throws<NotSupportedException>(() => sut.Transform(objectsToTransform, configuration));
        }

        private static IEnumerable<SampleClass> CreateObjectToTransform()
        {
            var objectToTransformOne = new SampleClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped"
            };

            var objectToTransformTwo = new SampleClass
            {
                Id = 2,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow.AddDays(1),
                NotMappedValue = "Should be not mapped"
            };

            return new List<SampleClass>()
            {
                objectToTransformOne,
                objectToTransformTwo
            };
        }

        private Configuration CreateConfiguration()
        {
            var conf = new Configuration();
            var mapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime);
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