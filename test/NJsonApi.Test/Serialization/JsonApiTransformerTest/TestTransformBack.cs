using Newtonsoft.Json.Linq;
using NJsonApi.Test.TestModel;
using NJsonApi.Serialization;
using System.Collections.Generic;
using System;
using Xunit;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.Builders;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestTransformBack
    {
        [Fact]
        public void Transform_properties_with_reserverd_keyword()
        {
            var updateDocument = new UpdateDocument()
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "post",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"title", "someTitle" }
                    }
                }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var transformer = new JsonApiTransformerBuilder()
                .With(config)
                .Build();

            // Act
            var resultDelta = transformer.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("title"));
        }

        [Fact]
        public void Transform_UpdateDocument_To_Delta_TwoFields()
        {
            // Arrange
            var updateDocument = new UpdateDocument
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "post",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"title", "someTitle" },
                        {"authorId", "1234" },
                    }
                }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var sut = new JsonApiTransformer(null, null, config);


            // Act
            var resultDelta = sut.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("title"));
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("authorId"));
        }

        class PostUpdateOneField
        {
            public string Title { get; set; }
        }

        class PostUpdateTwoFields
        {
            public int AuthorId { get; set; }
            public string Title { get; set; }
        }
    }
}
