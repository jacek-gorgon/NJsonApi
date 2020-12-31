using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Representations.Resources;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace UtilJsonApiSerializer.Test.Serialization.JsonApiTransformerTest
{
    public class TestLinked
    {
        private const string appUrl = @"http://localhost/";

        private JsonApiTransformer transformer;
        private readonly TransformationHelper transformationHelper = new TransformationHelper();

        public TestLinked()
        {
            transformer = new JsonApiTransformer(){TransformationHelper = transformationHelper};
        }

        [Theory]
        public void Creates_one_to_one_relationship()
        {
            // Arrange
            var context = CreateContext();
            var objectToTransform = CreateObject();
           

            // Act
            var result = transformer.Transform(objectToTransform, context);

            // Assert
            var resource = (SingleResource)result.Data;
            resource.Relationships.Count.Should().Be(1);
        }

        [Theory]
        public void Creates_one_to_many_relation_links()
        {
            // Arrange
            var configuration = CreateOneToManyConfigurationContext();
            var objectToTransform = CreateOneToManyObject();

            // Act
            var result = transformer.Transform(objectToTransform, configuration);

            // Assert
            var linkedDict = result.Included.Should().NotBeEmpty();
        }

        [Theory]
        public void Creates_one_to_many_relation_links_without_duplication()
        {
            // Arrange
            var configuration = CreateOneToManyConfigurationContext();
            var objectToTransform = CreateOneToManyObject();

            // Act
            var result = transformer.Transform(objectToTransform, configuration);

            // Assert
            result.Included.Count.Should().Be(2);
        }

        private object CreateOneToManyObject()
        {
            var duplicated = new NestedClass()
            {
                Id = 1000,
                SomeNestedValue = "Nested text value"
            };

            var sampleClass = new SampleClass()
            {
                Id = 1,
                SomeValue = "Some string value",
                NestedClassId = 1000,
                NestedValue = duplicated,

                NestedClasses = new List<NestedClass>()
                {
                    duplicated,
                    new NestedClass()
                    {
                        Id = 1001,
                        SomeNestedValue = "Nested text value"
                    }
                }
            };
            return sampleClass;
        }

        private SampleClass CreateObject()
        {
            var sampleClass = new SampleClass()
            {
                Id = 1,
                SomeValue = "Some string value",
                NestedClassId = 1000,
                NestedValue = new NestedClass()
                {
                    Id = 1000,
                    SomeNestedValue = "Nested text value"
                }
            };

            return sampleClass;
        }

        private Context CreateContext()
        {
            var conf = new Configuration();
            var sampleClassMapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            sampleClassMapping.ResourceType = "sampleClasses";
            sampleClassMapping.AddPropertyGetter("someValue", c => c.SomeValue);
            sampleClassMapping.AddPropertyGetter("nestedValue", c => c.NestedValue);


            var nestedClassMapping = new ResourceMapping<NestedClass>(c => c.Id, "nested/{id}");
            nestedClassMapping.ResourceType = "nestedClasses";
            nestedClassMapping.AddPropertyGetter("someNestedValue", c => c.SomeNestedValue);

            var linkMapping = new LinkMapping<SampleClass, NestedClass>()
            {
                RelationshipName = "nestedValues",
                ResourceGetter = c => c.NestedValue,
                ResourceMapping = nestedClassMapping,
                ResourceIdGetter = c => c.NestedClassId,
            };

            sampleClassMapping.Relationships.Add(linkMapping);

            conf.AddMapping(sampleClassMapping);
            conf.AddMapping(nestedClassMapping);

            return new Context
            {
                Configuration = conf,
                RoutePrefix = appUrl
            };

        }

        private Context CreateOneToManyConfigurationContext()
        {
            var conf = new Configuration();
            var sampleClassMapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            sampleClassMapping.ResourceType = "sampleClasses";
            sampleClassMapping.AddPropertyGetter("someValue", c => c.SomeValue);

            var nestedClassMapping = new ResourceMapping<NestedClass>(c => c.Id, "http://nested/{id}");
            nestedClassMapping.ResourceType = "nestedClasses";
            nestedClassMapping.AddPropertyGetter("someNestedValue", c => c.SomeNestedValue);

            var linkMapping = new LinkMapping<SampleClass, NestedClass>()
            {
                RelatedBaseResourceType = "nestedClasses",
                RelationshipName = "nestedValues",
                ResourceGetter = c => c.NestedClasses,
                ResourceMapping = nestedClassMapping,
                IsCollection = true,      
            };

            sampleClassMapping.Relationships.Add(linkMapping);

            conf.AddMapping(sampleClassMapping);
            conf.AddMapping(nestedClassMapping);

            return new Context
            {
                Configuration = conf,
                RoutePrefix = appUrl
            };

        }

        class SampleClass
        {
            public int Id { get; set; }
            public string SomeValue { get; set; }
            public NestedClass NestedValue { get; set; }
            public IEnumerable<NestedClass> NestedClasses { get; set; }
            public int NestedClassId { get; set; }
        }

        class NestedClass
        {
            public int Id { get; set; }
            public string SomeNestedValue { get; set; }
        }
    }


}