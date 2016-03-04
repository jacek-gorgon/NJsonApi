using NJsonApi.Serialization;
using NJsonApi.Test.Builders;
using NJsonApi.Test.Fakes;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NJsonApi.Test.Serialization
{
    public class LinkBuilderTests
    {
        private readonly IConfiguration configuration;
        private readonly Context context;

        public LinkBuilderTests()
        {
            var configBuilder = TestModelConfigurationBuilder.BuilderForEverything;
            configuration = configBuilder.Build();
            context = new Context(new Uri("http://www.example.com"));
        }

        [Fact]
        public void GIVEN_PostResource_WHEN_SelfLink_THEN_CorrectLink()
        {
            // Arrange
            var linkBuilder = GetLinkBuilder();
            var resourceMap = configuration.GetMapping(typeof(Post));

            // Act
            var result = linkBuilder.FindResourceSelfLink(context, "1", resourceMap);

            // Assert
            Assert.Equal("http://www.example.com/posts/1", result.Href);
        }


        [Fact]
        public void GIVEN_PostResource_AND_LinkMapToAuthor_WHEN_RelationshipSelf_THEN_CorrectLink()
        {
            // Arrange
            var linkBuilder = GetLinkBuilder();
            var resourceMap = configuration.GetMapping(typeof(Post));
            var relationship = resourceMap.Relationships.Single(x => x.RelatedBaseResourceType == "authors");

            // Act
            var result = linkBuilder.RelationshipSelfLink(context, "1", resourceMap, relationship);

            // Assert
            Assert.Equal("http://www.example.com/posts/1/relationships/author", result.Href);
        }

        [Fact]
        public void GIVEN_PostResource_AND_LinkMapToAuthor_WHEN_RelationshipRelated_THEN_CorrectLink()
        {
            // Arrange
            var linkBuilder = GetLinkBuilder();
            var resourceMap = configuration.GetMapping(typeof(Post));
            var relationship = resourceMap.Relationships.Single(x => x.RelatedBaseResourceType == "authors");

            // Act
            var result = linkBuilder.RelationshipRelatedLink(context, "1", resourceMap, relationship);

            // Assert
            Assert.Equal("http://www.example.com/posts/1/author", result.Href);
        }

        private LinkBuilder GetLinkBuilder()
        {
            var provider = new FakeApiDescriptionGroupCollectionProvider();
            provider
                .WithPostsController()
                .WithGetAction();

            return new LinkBuilder(
                provider,
                new FakeUrlHelper());
        }
    }
}
