using Newtonsoft.Json;
using NJsonApi.Serialization;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestSerialization
    {
        [Fact]
        public void Serilized_properly()
        {
            // Arrange
            var configuration = CreateContext();
            var objectToTransform = CreateObjectToTransform();
            var sut = new JsonApiTransformer();

            var transformed = sut.Transform(objectToTransform, configuration);

            // Act
            var json = JsonConvert.SerializeObject(transformed);

            // Assert
            Assert.DoesNotContain(json, "Data");
        }

        private static SampleClass CreateObjectToTransform()
        {
            var deepest = new DeeplyNestedClass()
            {
                Id = 100,
                Value = "A value"
            };

            return new SampleClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped",
                NestedClass = new List<NestedClass>()
                {
                    new NestedClass()
                    {
                        Id = 200,
                        NestedText = "Some nested text",
                        DeeplyNestedClass = deepest
                    },
                    new NestedClass()
                    {
                        Id = 200,
                        NestedText = "Some nested text",
                        DeeplyNestedClass = deepest
                    },
                    new NestedClass()
                    {
                        Id = 201,
                        NestedText = "Some nested text"
                    }
                }
            };
        }

        private Context CreateContext()
        {
            var conf = new NJsonApi.Configuration();
            var sampleClassMapping = new ResourceMapping<SampleClass>(c => c.Id, "http://sampleClass/{id}");
            sampleClassMapping.ResourceType = "sampleClasses";
            sampleClassMapping.AddPropertyGetter("someValue", c => c.SomeValue);
            sampleClassMapping.AddPropertyGetter("date", c => c.DateTime);


            var nestedClassMapping = new ResourceMapping<NestedClass>(c => c.Id, "http://nestedclass/{id}");
            nestedClassMapping.ResourceType = "nestedClasses";
            nestedClassMapping.AddPropertyGetter("nestedText", c => c.NestedText);


            var deeplyNestedMapping = new ResourceMapping<DeeplyNestedClass>(c => c.Id, "http://deep/{id}");
            deeplyNestedMapping.ResourceType = "deeplyNestedClasses";
            deeplyNestedMapping.AddPropertyGetter("value", c => c.Value);


            var linkMapping = new RelationshipMapping<SampleClass, NestedClass>()
            {
                IsCollection = true,
                RelationshipName = "nestedValues",
                ResourceMapping = nestedClassMapping,
                ResourceGetter = c => c.NestedClass,
            };

            var deepLinkMapping = new RelationshipMapping<NestedClass, DeeplyNestedClass>()
            {
                RelationshipName = "deeplyNestedValues",
                ResourceMapping = deeplyNestedMapping,
                ResourceGetter = c => c.DeeplyNestedClass
            };

            sampleClassMapping.Relationships.Add(linkMapping);
            nestedClassMapping.Relationships.Add(deepLinkMapping);

            conf.AddMapping(sampleClassMapping);
            conf.AddMapping(nestedClassMapping);
            conf.AddMapping(deeplyNestedMapping);

            return new Context(conf, new Uri("http://fakehost:1234/", UriKind.Absolute));
        }

        class SampleClass
        {
            public int Id { get; set; }
            public string SomeValue { get; set; }
            public DateTime DateTime { get; set; }
            public string NotMappedValue { get; set; }
            public IEnumerable<NestedClass> NestedClass { get; set; }
        }

        class NestedClass
        {
            public int Id { get; set; }
            public string NestedText { get; set; }
            public DeeplyNestedClass DeeplyNestedClass { get; set; }
        }

        private class DeeplyNestedClass
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}