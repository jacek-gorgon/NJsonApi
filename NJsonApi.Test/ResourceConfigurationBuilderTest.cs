using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialCee.Framework.NJsonApi.Test.TestModel;
using SoftwareApproach.TestingExtensions;

namespace SocialCee.Framework.NJsonApi.Test
{
    [TestClass]
    public class ResourceConfigurationBuilderTest
    {
        private ConfigurationBuilder configurationBuilder;

        [TestInitialize]
        public void Init()
        {
            configurationBuilder = new ConfigurationBuilder();
        }

        [TestMethod]
        public void TestWithResourceType()
        {
            //Arrange
            string resourceType = typeof(Author).Name;
            var classUnderTest = configurationBuilder.Resource<Author>();
            //Act
            classUnderTest.WithResourceType(resourceType);
            //Assert
            classUnderTest.ConstructedMetadata.ResourceType.ShouldEqual(resourceType);
        }

        [TestMethod]
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
                .ShouldEqual(resourceTypeAuthor);

            resourceConfigurationForPost
                .ConstructedMetadata
                .ResourceType
                .ShouldEqual(resourceTypePost);
        }

        [TestMethod]
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
            result.ShouldEqual(authorId);
        }

        [TestMethod]
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
            authorResult.ShouldEqual(authorId);
            var postResult = (int)resourceConfigurationForPost.ConstructedMetadata.IdGetter.Invoke(post);
            postResult.ShouldEqual(postId);
        }

        [TestMethod]
        public void TestWithSimpleProperty()
        {
            //Arrange
            const int authorId = 5;
            var author = new Author() { Id = authorId };
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            //Act
            resourceConfigurationForAuthor.WithSimpleProperty(a => a.Name);
            //Assert
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters.Count.ShouldEqual(1);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertySetters.Count.ShouldEqual(1);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.ShouldBeNull();
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.ShouldContain(typeof(Author).Name.ToLower());
        }

        class ClassWithReserveredKeys
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Links { get; set; }
            public string Href { get; set; }
        }

        [TestMethod]
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
            propertyGetters.Keys.ShouldContain("_id").ShouldNotContain("id");
            propertyGetters.Keys.ShouldContain("_type").ShouldNotContain("type");
            propertyGetters.Keys.ShouldContain("_links").ShouldNotContain("links");
            propertyGetters.Keys.ShouldContain("_href").ShouldNotContain("href");

            var propertySetters = sut.ConstructedMetadata.PropertySetters;
            propertySetters.Keys.ShouldContain("_id").ShouldNotContain("id");
            propertySetters.Keys.ShouldContain("_type").ShouldNotContain("type");
            propertySetters.Keys.ShouldContain("_links").ShouldNotContain("links");
            propertySetters.Keys.ShouldContain("_href").ShouldNotContain("href");
        }

        [TestMethod]
        public void TestWithSimplePropertyWithIdentity()
        {
            //Arrange & Act
            var resourceConfigurationForAuthor = configurationBuilder
                .Resource<Author>()
                .WithSimpleProperty(a => a.Name)
                .WithIdSelector(a => a.Id);
            //Assert
            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForAuthor);
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.ShouldContain(typeof(Author).Name.ToLower());
        }

        [TestMethod]
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
            resultForName.ShouldEqual(authorName);
            resultForId.ShouldEqual(authorId);
        }

        [TestMethod]
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
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.ShouldContain(typeof(Author).Name.ToLower());
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters["name"].Invoke(author).ShouldEqual(authorName);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.Invoke(author).ShouldEqual(authorId);

            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForPost);
            resourceConfigurationForPost.ConstructedMetadata.ResourceType.ShouldContain(typeof(Post).Name.ToLower());
            resourceConfigurationForPost.ConstructedMetadata.PropertyGetters["title"].Invoke(post).ShouldEqual(postTitle);
            resourceConfigurationForPost.ConstructedMetadata.PropertySetters["title"].Invoke(post, postTitleModifed);
            post.Title.ShouldEqual(postTitleModifed);
            resourceConfigurationForPost.ConstructedMetadata.PropertyGetters["title"].Invoke(post).ShouldEqual(postTitleModifed);
        }

        [TestMethod]
        public void IgnorePropertyTest()
        {
            //Arrange
            const int authorId = 5;
            var author = new Author() { Id = authorId };
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            resourceConfigurationForAuthor.WithSimpleProperty(a => a.Name);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters.Count.ShouldEqual(1);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertySetters.Count.ShouldEqual(1);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.ShouldBeNull();
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.ShouldContain(typeof(Author).Name.ToLower());
            //Act
            resourceConfigurationForAuthor.IgnoreProperty(a => a.Name);
            //Assert
            resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters.Count.ShouldEqual(0);
            resourceConfigurationForAuthor.ConstructedMetadata.PropertySetters.Count.ShouldEqual(0);
            resourceConfigurationForAuthor.ConstructedMetadata.IdGetter.ShouldBeNull();
            resourceConfigurationForAuthor.ConstructedMetadata.ResourceType.ShouldContain(typeof(Author).Name.ToLower());

        }

        [TestMethod]
        public void WithLinkedResourceTest()
        {
            //Arrange

            //Act
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithIdSelector(p => p.Id)
                .WithSimpleProperty(p => p.Title)
                .WithLinkedResource<Author>(p => p.Author);

            //Assert
            resourceConfigurationForPost.ConstructedMetadata.Links.Count.ShouldEqual(1);
            resourceConfigurationForPost.ConstructedMetadata.Links[0].LinkName.ShouldContain(typeof(Author).Name.ToLower());
            resourceConfigurationForPost.ConstructedMetadata.Links[0].LinkedType.ShouldEqual(typeof(Author));
            resourceConfigurationForPost.ConstructedMetadata.Links[0].ResourceMapping.ShouldBeNull();
        }

        [TestMethod]
        public void WithLinkTemplateTest()
        {
            //Arrange
            const string urlTemplate = "urlTemplate";
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithIdSelector(p => p.Id)
                .WithSimpleProperty(p => p.Title);
            resourceConfigurationForPost.ConstructedMetadata.UrlTemplate.ShouldBeNull();
            //Act
            resourceConfigurationForPost
                 .WithLinkTemplate(urlTemplate);

            //Assert

            resourceConfigurationForPost.ConstructedMetadata.UrlTemplate.ShouldEqual(urlTemplate);
        }



        private void AssertResourceConfigurationHasValuesForWithSimpleProperty(IResourceConfigurationBuilder resourceConfiguration)
        {
            resourceConfiguration.ConstructedMetadata.PropertyGetters.Count.ShouldEqual(1);
            resourceConfiguration.ConstructedMetadata.PropertySetters.Count.ShouldEqual(1);
            resourceConfiguration.ConstructedMetadata.IdGetter.ShouldNotBeNull();
        }
    }
}
