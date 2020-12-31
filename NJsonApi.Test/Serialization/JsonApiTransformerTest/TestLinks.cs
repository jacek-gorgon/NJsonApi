using FluentAssertions;
using NUnit.Framework;
using UtilJsonApiSerializer.Serialization;
using UtilJsonApiSerializer.Serialization.Representations.Resources;
using UtilJsonApiSerializer.Serialization.Representations.Relationships;

namespace UtilJsonApiSerializer.Test.Serialization.JsonApiTransformerTest
{
    public class TestLinks
    {
        private const string appUrl = @"http://localhost/";

        private JsonApiTransformer transformer;
        private readonly TransformationHelper transformationHelper = new TransformationHelper();

        public TestLinks()
        {
            transformer = new JsonApiTransformer(){TransformationHelper = transformationHelper};
        }

        [Theory]
        public void Creates_one_to_one_relation_links()
        {
            // Arrange
            var context = CreateContext();
            var objectToTransform = CreateObject();

            // Act
            var result = transformer.Transform(objectToTransform, context);
            var resource = (SingleResource)result.Data;

            // Assert
            resource.Relationships.Should().NotBeEmpty();
            ((Relationship)resource.Relationships["nestedValues"]).Data.Should().NotBeNull();
        }

        [Theory]
        public void Creates_one_to_one_null_relation_links()
        {
            // Arrange
            var context = CreateContext();
            var objectToTransform = CreateNullNestedObject();

            // Act
            var result = transformer.Transform(objectToTransform, context);
            var resource = (SingleResource)result.Data;

            // Assert
            resource.Relationships.Should().NotBeEmpty();
            var rel = (Relationship)resource.Relationships["nestedValues"];
            rel.Data.Should().BeOfType(typeof(NullResourceIdentifier));
        }


        private SampleClass CreateObject()
        {
            var sampleClass = new SampleClass()
            {
                Id = 1,
                SomeValue = "Some string value",
                NestedValue = new NestedClass()
                {
                    Id = 1000,
                    SomeNestedValue = "Nested text value"
                }
            };

            return sampleClass;
        }

        private SampleClass CreateNullNestedObject()
        {
            var sampleClass = new SampleClass()
            {
                Id = 1,
                SomeValue = "Some string value",
                NestedValue = null
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
                ResourceMapping = nestedClassMapping,
                ResourceGetter = c => c.NestedValue,
                ResourceIdGetter = c => c.NestedValueId
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
            public int? NestedValueId { get; set; }
            public NestedClass NestedValue { get; set; }
        }

        class NestedClass
        {
            public int Id { get; set; }
            public string SomeNestedValue { get; set; }
        }
    }


}