using System;
using System.Collections.Generic;
using NJsonApi.Common.Infrastructure;
using NJsonApi.Serialization;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations.Resources;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestMetadataWrapper
    {
        readonly List<string> reservedKeys = new List<string> { "id", "type", "href", "links" };

        [Fact]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            var context = CreateContext();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            Assert.NotNull(result.Data);
            var transformedObject = result.Data as SingleResource;
            Assert.NotNull(transformedObject);
        }

        [Fact]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            var context = CreateContext();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Id, objectToTransform.Value.Id.ToString());
        }

        [Fact]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            var context = CreateContext();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };


            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Attributes["someValue"], objectToTransform.Value.SomeValue);
            Assert.Equal(transformedObject.Attributes["date"], objectToTransform.Value.DateTime);
            Assert.Equal(transformedObject.Attributes.Count, 2);
        }

        [Fact]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_href()
        {
            // Arrange
            var context = CreateContext();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut
                .Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Links["self"].ToString(), "http://sampleclass/1");
        }

        [Fact]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            var context = CreateContext();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Type, "sampleClasses");
        }

        [Fact]
        public void Creates_CompondDocument_for_metadatawrapper_single_not_nested_class_and_propertly_map_metadata()
        {
            // Arrange
            const string pagingValue = "1";
            const string countValue = "2";

            var context = CreateContext();
            MetaDataWrapper<SampleClass> objectToTransform = CreateObjectToTransform();
            objectToTransform.MetaData.Add("Paging", pagingValue);
            objectToTransform.MetaData.Add("Count", countValue);
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };


            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObjectMetadata = result.Meta;
            Assert.Equal(transformedObjectMetadata["Paging"], pagingValue);
            Assert.Equal(transformedObjectMetadata["Count"], countValue);
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

        private Context CreateContext()
        {
            var conf = new NJsonApi.Configuration();
            var mapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime);
            conf.AddMapping(mapping);

            return new Context(conf, new Uri("http://fakehost:1234/", UriKind.Absolute));
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