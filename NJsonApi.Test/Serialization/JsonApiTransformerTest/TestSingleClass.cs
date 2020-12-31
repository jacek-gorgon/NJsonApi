using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Documents;
using UtilJsonApiSerializer.Serialization.Representations.Resources;
using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace UtilJsonApiSerializer.Test.Serialization.JsonApiTransformerTest
{
    public class TestSingleClass
    {
        readonly List<string> reservedKeys = new List<string> { "id", "type", "href", "links" };
           
        [Theory]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            result.Data.Should().NotBeNull();
            var transformedObject = result.Data as SingleResource;
            transformedObject.Should().NotBeNull();
        }

        [Theory]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            transformedObject.Id.Should().Be(objectToTransform.Id.ToString());
        }

        [Theory]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            transformedObject.Attributes["someValue"].Should().Be(objectToTransform.SomeValue);
            transformedObject.Attributes["date"].Should().Be(objectToTransform.DateTime);
            transformedObject.Attributes.Count.Should().Be(2);
        }

        [Theory]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_href()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            transformedObject.Links["self"].ToString().Should().Be("http://sampleclass/1");
        }

        [Theory]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            transformedObject.Type.Should().Be("sampleClasses");
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

        private Context CreateContext()
        {
            var conf = new Configuration();
            var mapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime );
            conf.AddMapping(mapping);

            return new Context
            {
                Configuration = conf,
                RoutePrefix = string.Empty
            };

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