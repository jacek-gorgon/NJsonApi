using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using UtilJsonApiSerializer.Serialization;
using SoftwareApproach.TestingExtensions;
using System.Collections.Generic;
using UtilJsonApiSerializer.Test.TestModel;

namespace UtilJsonApiSerializer.Test.Serialization.JsonApiTransformerTest
{
    [TestClass]
    public class TestTransformBack
    {
        [TestMethod]
        public void Transform_properties_with_reserverd_keyword()
        {
            var updateDocument = new UpdateDocument()
            {
                Data = new Dictionary<string, object>()
                {
                    { "posts", JObject.Parse("{ \"_id\":123, \"title\": \"someTitle\" }") }
                }
            };

            var configuration = (new ConfigurationBuilder())
                .Resource<Post>()
                .WithSimpleProperty(x => x.AuthorId)
                .WithSimpleProperty(x => x.Id)
                .WithSimpleProperty(x => x.Title);
            var context = new Context { Configuration = configuration.ConfigurationBuilder.Build() };
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            var resultDelta = sut.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            resultDelta.ObjectPropertyValues.ContainsKey("id").ShouldBeTrue();
        }

        [TestMethod]
        public void Transform_UpdateDocument_To_Delta_OneField()
        {
            // Arrange
            var updateDocument = new UpdateDocument
            {
                Data = new Dictionary<string, object>
                {
                    {
                        "posts", JObject.FromObject(new PostUpdateOneField()
                        {
                            Title = "Food"
                        })
                    }
                }
            };

            var configuration = (new ConfigurationBuilder())
                .Resource<Post>()
                .WithSimpleProperty(x => x.AuthorId)
                .WithSimpleProperty(x => x.Id)
                .WithSimpleProperty(x => x.Title);
            var context = new Context { Configuration = configuration.ConfigurationBuilder.Build() };
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };

            // Act
            var resultDelta = sut.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            resultDelta.ObjectPropertyValues.ContainsKey("title").ShouldBeTrue();
        }

        [TestMethod]
        public void Transform_UpdateDocument_To_Delta_TwoFields()
        {
            // Arrange
            var updateDocument = new UpdateDocument
            {
                Data = new Dictionary<string, object>
                {
                    {
                        "posts", JObject.FromObject(new PostUpdateTwoFields()
                        {
                            Title = "Food",
                            AuthorId = 0
                        })
                    }
                }
            };

            var configuration = (new ConfigurationBuilder())
                .Resource<Post>()
                .WithSimpleProperty(x => x.AuthorId)
                .WithSimpleProperty(x => x.Title);
            var context = new Context { Configuration = configuration.ConfigurationBuilder.Build() };
            var sut = new JsonApiTransformer() { TransformationHelper = new TransformationHelper() };


            // Act
            var resultDelta = sut.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            resultDelta.ObjectPropertyValues.ContainsKey("title").ShouldBeTrue();
            resultDelta.ObjectPropertyValues.ContainsKey("authorId").ShouldBeTrue();
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
