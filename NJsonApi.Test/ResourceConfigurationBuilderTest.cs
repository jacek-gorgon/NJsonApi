using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UtilJsonApiSerializer.Test.TestModel;

namespace UtilJsonApiSerializer.Test
{
    public class ResourceConfigurationBuilderTest
    {
        private ConfigurationBuilder configurationBuilder;

        [SetUp]
        public void Init()
        {
            configurationBuilder = new ConfigurationBuilder();
        }

        [Theory]
        public void TestWithResourceType()
        {
            //Arrange
            string resourceType = typeof(Author).Name;
            var classUnderTest = configurationBuilder.Resource<Author>();
            //Act
            classUnderTest.WithResourceType(resourceType);
            //Assert
            classUnderTest.ConstructedMetadata.ResourceType.Should().Be(resourceType);
        }

        [Theory]
        public void TestWithResourceTypeForMultipleTypes()
        {
            //Arrange
            string resourceTypeAuthor = typeof(Author).Name;
            string resourceTypePost = typeof(Post).Name;
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            var resourceConfigurationForPost = configurationBuilder.Resource<Post>();
            //Act
            resourceConfigurationForAuthor.WithResourceType(resourceTypeAuthor);
            resourceConfigurationForPost.WithResourceType(resourceTypePost);
            //Assert
            resourceConfigurationForAuthor
                .ConstructedMetadata
                .ResourceType
                .Should().Be(resourceTypeAuthor);

            resourceConfigurationForPost
                .ConstructedMetadata
                .ResourceType
                .Should().Be(resourceTypePost);
        }

        [Theory]
        public void TestWithIdSelector()
        {
            //Arrange
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            const int authorId = 5;
            var author = new Author() { Id = authorId };
            //Act
            resourceConfigurationForAuthor.WithIdSelector(a => a.Id);
            //Assert
            var result = (int)resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Invoke(author);
            result.Should().Be(authorId);
        }

        [Theory]
        public void TestWithIdSelectorForMultipleTypes()
        {
            //Arrange
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            var resourceConfigurationForPost = configurationBuilder.Resource<Post>();
            const int authorId = 5;
            const int postId = 6;
            var author = new Author() { Id = authorId };
            var post = new Post() { Id = postId };
            //Act
            resourceConfigurationForAuthor.WithIdSelector(a => a.Id);
            resourceConfigurationForPost.WithIdSelector(p => p.Id);
            //Assert
            var authorResult = (int)resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Invoke(author);
            authorResult.Should().Be(authorId);
            var postResult = (int)resourceConfigurationForPost.ConstructedMetadata.IdGetter.Invoke(post);
            postResult.Should().Be(postId);
        }

        [Theory]
        public void TestWithSimpleProperty()
        {
            //Arrange
            const int authorId = 5;
            var author = new Author() { Id = authorId };
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            //Act
            resourceConfigurationForAuthor.WithSimpleProperty(a => a.Name);
            //Assert
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters.Count.Should().Be(1);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertySetters.Count.Should().Be(1);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Should().BeNull();
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.Contains(typeof(Author).Name.ToLower()).Should().BeTrue();
        }

        class ClassWithReserveredKeys
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Links { get; set; }
            public string Href { get; set; }
        }

        [Theory]
        public void TestWithSimpleProperty_with_reserved_jsonapi_key()
        {
            // Arrange
            var sut = configurationBuilder.Resource<ClassWithReserveredKeys>();

            // Act
            sut.WithSimpleProperty(a => a.Id);
            sut.WithSimpleProperty(a => a.Type);
            sut.WithSimpleProperty(a => a.Links);
            sut.WithSimpleProperty(a => a.Href);

            // Assert
            var propertyGetters = sut.ConstructedMetadata.PropertyGetters;
            propertyGetters.Keys.Should().Contain("_id");
            propertyGetters.Keys.Should().NotContain("id");
            propertyGetters.Keys.Should().Contain("_type");
            propertyGetters.Keys.Should().NotContain("type");
            propertyGetters.Keys.Should().Contain("_links");
            propertyGetters.Keys.Should().NotContain("links");
            propertyGetters.Keys.Should().Contain("_href");
            propertyGetters.Keys.Should().NotContain("href");

            var propertySetters = sut.ConstructedMetadata.PropertySetters;
            propertySetters.Keys.Should().Contain("_id");
            propertySetters.Keys.Should().NotContain("id");
            propertySetters.Keys.Should().Contain("_type");
            propertySetters.Keys.Should().NotContain("type");
            propertySetters.Keys.Should().Contain("_links");
            propertySetters.Keys.Should().NotContain("links");
            propertySetters.Keys.Should().Contain("_href");
            propertySetters.Keys.Should().NotContain("href");
        }

        [Theory]
        public void TestWithSimplePropertyWithIdentity()
        {
            //Arrange & Act
            var resourceConfigurationForAuthor = configurationBuilder
                .Resource<Author>()
                .WithSimpleProperty(a => a.Name)
                .WithIdSelector(a => a.Id);
            //Assert
            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForAuthor);
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.Should().Contain(typeof(Author).Name.ToLower());
        }

        [Theory]
        public void TestWithSimplePropertyWithIdentyAndAscessor()
        {
            //Arrange
            const int authorId = 5;
            const string authorName = "Valentin";
            var author = new Author() { Id = authorId, Name = authorName };

            //Act
            var resourceConfigurationForAuthor = configurationBuilder
                .Resource<Author>()
                .WithSimpleProperty(a => a.Name)
                .WithIdSelector(a => a.Id);

            var resultForName = resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters["name"].Invoke(author);
            var resultForId = resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Invoke(author);

            //Assert
            resultForName.Should().Be(authorName);
            resultForId.Should().Be(authorId);
        }

        [Theory]
        public void TestWithSimplePropertyMultipleTypes()
        {
            //Arrange
            const int authorId = 5;
            const string authorName = "Valentin";
            const int postId = 6;
            const string postTitle = "The measure of a man";
            const string postTitleModifed = "Modified";
            var author = new Author() { Id = authorId, Name = authorName };
            var post = new Post() { Id = postId, Title = postTitle };

            //Act
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithSimpleProperty(p => p.Title)
                .WithIdSelector(p => p.Id);
            var resourceConfigurationForAuthor = configurationBuilder
                .Resource<Author>()
                .WithSimpleProperty(a => a.Name)
                .WithIdSelector(a => a.Id);

            //Assert
            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForAuthor);
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.Should().Contain(typeof(Author).Name.ToLower());
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters["name"].Invoke(author).Should().Be(authorName);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Invoke(author).Should().Be(authorId);

            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForPost);
            resourceConfigurationForPost.ConstructedMetadata.ResourceType.Should().Contain(typeof(Post).Name.ToLower());
            resourceConfigurationForPost.ConstructedMetadata.PropertyGetters["title"].Invoke(post).Should().Be(postTitle);
            resourceConfigurationForPost.ConstructedMetadata.PropertySetters["title"].Invoke(post, postTitleModifed);
            post.Title.Should().Be(postTitleModifed);
            resourceConfigurationForPost.ConstructedMetadata.PropertyGetters["title"].Invoke(post).Should().Be(postTitleModifed);
        }

        [Theory]
        public void IgnorePropertyTest()
        {
            //Arrange
            const int authorId = 5;
            var author = new Author() { Id = authorId };
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            resourceConfigurationForAuthor.WithSimpleProperty(a => a.Name);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters.Count.Should().Be(1);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertySetters.Count.Should().Be(1);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Should().BeNull();
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.Should().Contain(typeof(Author).Name.ToLower());
            //Act
            resourceConfigurationForAuthor.IgnoreProperty(a => a.Name);
            //Assert
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters.Count.Should().Be(0);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertySetters.Count.Should().Be(0);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Should().BeNull();
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.Should().Contain(typeof(Author).Name.ToLower());

        }

        [Theory]
        public void WithLinkedResourceTest()
        {
            //Arrange
            
            //Act
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithIdSelector(p => p.Id)
                .WithSimpleProperty(p => p.Title)
                .WithLinkedResource<Author>(p => p.Author,null, null, "author", ResourceInclusionRules.Smart, null, "author");

            //Assert
            resourceConfigurationForPost.ConstructedMetadata.Relationships.Count.Should().Be(1);
            resourceConfigurationForPost.ConstructedMetadata.Relationships[0].RelationshipName.Should().Contain(typeof(Author).Name.ToLower());
            resourceConfigurationForPost.ConstructedMetadata.Relationships[0].RelatedBaseType.Should().Be(typeof(Author));
            resourceConfigurationForPost.ConstructedMetadata.Relationships[0].ResourceMapping.Should().BeNull();
        }

        [Theory]
        public void WithLinkTemplateTest()
        {
            //Arrange
            const string urlTemplate = "urlTemplate";
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithIdSelector(p => p.Id)
                .WithSimpleProperty(p => p.Title);
            resourceConfigurationForPost.ConstructedMetadata.UrlTemplate.Should().BeNull();
            //Act
            resourceConfigurationForPost
                 .WithLinkTemplate(urlTemplate);

            //Assert

            resourceConfigurationForPost.ConstructedMetadata.UrlTemplate.Should().Be(urlTemplate);
        }



        private void AssertResourceConfigurationHasValuesForWithSimpleProperty(IResourceConfigurationBuilder resourceConfiguration)
        {
            resourceConfiguration.ConstructedMetadata.PropertyGetters.Count.Should().Be(1);
            resourceConfiguration.ConstructedMetadata.PropertySetters.Count.Should().Be(1);
            resourceConfiguration.ConstructedMetadata.IdGetter.Should().NotBeNull();
        }
    }
}
