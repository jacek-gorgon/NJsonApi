using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Common.Infrastructure;
using NJsonApi.Serialization;
using SoftwareApproach.TestingExtensions;
using NJsonApi.Serialization.Documents;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    [TestClass]
    public class TestMetadataWrapper
    {
        readonly List<string> reservedKeys = new List<string> { "id", "type", "href", "links" };

        [TestMethod]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            result.Data.ShouldNotBeNull();
            var transformedObject = result.Data as Dictionary<string, object>;
            transformedObject.ShouldNotBeNull();
        }

        [TestMethod]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data as Dictionary<string, object>;
            transformedObject["id"].ShouldEqual(objectToTransform.Value.Id.ToString());
        }

        [TestMethod]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };


            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data as Dictionary<string, object>;
            transformedObject["someValue"].ShouldEqual(objectToTransform.Value.SomeValue);
            transformedObject["date"].ShouldEqual(objectToTransform.Value.DateTime);
            transformedObject.Keys.Where(k => !reservedKeys.Contains(k)).ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_href()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut
                .Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data as Dictionary<string, object>;
            transformedObject["href"].ShouldEqual("http://sampleclass/1");
        }

        [TestMethod]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            Configuration configuration = CreateConfiguration();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObject = result.Data as Dictionary<string, object>;
            transformedObject["type"].ShouldEqual("sampleClasses");
        }

        [TestMethod]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_metadata()
        {
            // Arrange
            const string pagingValue = "1";
            const string countValue = "2";

            Configuration configuration = CreateConfiguration();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            objectToTransform.MetaData.Add("Paging", pagingValue);
            objectToTransform.MetaData.Add("Count", countValue);
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };


            // Act
            CompoundDocument result = sut.Transform(objectToTransform, configuration);

            // Assert
            var transformedObjectMetadata = result.Metadata;
            transformedObjectMetadata["Paging"].ShouldEqual(pagingValue);
            transformedObjectMetadata["Count"].ShouldEqual(countValue);
        }



        private static MetaDataWrapper<SampleClass> CreateObjectToTransform()
        {
            var objectToTransform = new SampleClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped"
            };
            return new MetaDataWrapper<SampleClass>(objectToTransform);
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