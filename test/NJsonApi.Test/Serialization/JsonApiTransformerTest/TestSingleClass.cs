using NJsonApi.Serialization;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.Builders;
using NJsonApi.Test.TestControllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestSingleClass
    {
        readonly List<string> reservedKeys = new List<string> { "id", "type", "href", "links" };
           
        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            Assert.NotNull(result.Data);
            var transformedObject = result.Data as SingleResource;
            Assert.NotNull(transformedObject);
        }

        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Id, objectToTransform.Id.ToString());
        }

        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Attributes["someValue"], objectToTransform.SomeValue);
            Assert.Equal(transformedObject.Attributes["date"], objectToTransform.DateTime);
            Assert.Equal(transformedObject.Attributes.Count, 2);
        }

        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Type, "sampleClasses");
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
            return new Context(new Uri("http://fakehost:1234/", UriKind.Absolute));
        }

        private IConfiguration CreateConfiguration()
        {
            var mapping = new ResourceMapping<SampleClass, DummyController>(c => c.Id, "http://sampleClass/{id}");
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime);

            var config = new NJsonApi.Configuration();
            config.AddMapping(mapping);
            return config; 
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