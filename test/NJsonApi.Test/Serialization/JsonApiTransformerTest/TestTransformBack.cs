using Newtonsoft.Json.Linq;
using NJsonApi.Test.TestModel;
using NJsonApi.Serialization;
using System.Collections.Generic;
using System;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestTransformBack
    {
        [Fact]
        public void Transform_properties_with_reserverd_keyword()
        {
            var updateDocument = new UpdateDocument()
            {
                Data = new Dictionary<string, object>()
                {
                    { "data", JObject.Parse("{ \"id\":123, \"type\":\"post\", \"attributes\" : { \"title\": \"someTitle\" }}") }
                }
            };

            var configuration = (new ConfigurationBuilder())
                .Resource<Post>()
                .WithSimpleProperty(x => x.AuthorId)
                .WithSimpleProperty(x => x.Id)
                .WithSimpleProperty(x => x.Title);
            var context = new Context(configuration.ConfigurationBuilder.Build(), new Uri("http://fakehost:1234", UriKind.Absolute));
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            var resultDelta = sut.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("title"));
        }

        [Fact]
        public void Transform_UpdateDocument_To_Delta_TwoFields()
        {
            // Arrange
            var updateDocument = new UpdateDocument
            {
                Data = new Dictionary<string, object>()
                {
                    { "data", JObject.Parse("{ \"id\":123, \"type\":\"post\", \"attributes\" : { \"title\": \"someTitle\", \"authorId\" : \"1234\"}}") }
                }
            };

            var configuration = (new ConfigurationBuilder())
                .Resource<Post>()
                .WithSimpleProperty(x => x.AuthorId)
                .WithSimpleProperty(x => x.Title);
            var context = new Context(configuration.ConfigurationBuilder.Build(), new Uri("http://fakehost:1234", UriKind.Absolute));
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };


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
