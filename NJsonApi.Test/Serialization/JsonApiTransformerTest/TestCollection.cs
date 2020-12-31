using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using UtilJsonApiSerializer.Common.Infrastructure;
using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Documents;
using UtilJsonApiSerializer.Serialization.Representations;
using UtilJsonApiSerializer.Serialization.Representations.Resources;

namespace UtilJsonApiSerializer.Test.Serialization.JsonApiTransformerTest
{
    public class TestCollection
    {
        readonly List<string> reservedKeys = new List<string> { "id", "type", "href", "links" };
           
        [Theory]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            var configuration = CreateContext();            
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            result.Data.Should().NotBeNull();
            var transformedObject = result.Data as ResourceCollection;
            transformedObject.Should().NotBeNull();
        }

        [Theory]
        public void Creates_CompondDocument_for_collection_not_nested_single_class()
        {
            // Arrange
            var configuration = CreateContext();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform().Take(1);
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            result.Data.Should().NotBeNull();
            var transformedObject = result.Data as ResourceCollection;
            transformedObject.Should().NotBeNull();
        }


        [Theory]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            var configuration = CreateContext();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data as ResourceCollection;
            transformedObject[0].Id.Should().Be(objectsToTransform.First().Id.ToString());
            transformedObject[1].Id.Should().Be(objectsToTransform.Last().Id.ToString());
        }

        [Theory]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            var configuration = CreateContext();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data as ResourceCollection;

            Action<SingleResource, SampleClass> assertSame = (actual, expected) =>
            {
                actual.Attributes["someValue"].Should().Be(expected.SomeValue);
                actual.Attributes["date"].Should().Be(expected.DateTime);
                actual.Attributes.Count.Should().Be(2);
            };

            assertSame(transformedObject[0], objectsToTransform.First());
            assertSame(transformedObject[1], objectsToTransform.Last());
        }

        [Theory]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_href()
        {
            // Arrange
            var configuration = CreateContext();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data as ResourceCollection;
            transformedObject[0].Links["self"].ToString().Should().Be("http://sampleclass/1");
            transformedObject[1].Links["self"].ToString().Should().Be("http://sampleclass/2");
        }

        [Theory]
        public void Creates_CompondDocument_for_collection_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            var configuration = CreateContext();
            IEnumerable<SampleClass> objectsToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            CompoundDocument result = sut.Transform(objectsToTransform, configuration);

            // Assert
            var transformedObject = result.Data as ResourceCollection;
            transformedObject[0].Type.Should().Be("sampleClasses");
            transformedObject[1].Type.Should().Be("sampleClasses");
        }

        [Theory]
        public void Creates_CompondDocument_for_collectione_of_metadatawrapper_throws_notSupportedException()
        {
            // Arrange
            var configuration = CreateContext();
            var objectsToTransform = new List<MetaDataWrapper<SampleClass>>
            {
                new MetaDataWrapper<SampleClass>(new SampleClass())
            };
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            Action act = () => sut.Transform(objectsToTransform, configuration);

            // Assert
            act.Should().Throw<NotSupportedException>();
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

        private Context CreateContext()
        {
            var conf = new Configuration();
            var mapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime);
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