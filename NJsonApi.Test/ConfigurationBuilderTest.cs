using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialCee.Framework.NJsonApi.Conventions;
using SocialCee.Framework.NJsonApi.Conventions.Impl;
using SocialCee.Framework.NJsonApi.Test.TestModel;
using SoftwareApproach.TestingExtensions;

namespace SocialCee.Framework.NJsonApi.Test
{
    [TestClass]
    public class ConfigurationBuilderTest
    {
        [TestMethod]
        public void Resource_creates_mapping()
        {
            //Arrange
            var builder = new ConfigurationBuilder();

            //Act
            builder.Resource<Post>();

            var result = builder.Build();

            //Assert
            result.IsMappingRegistered(typeof(Post)).ShouldBeTrue();
            result.GetMapping(typeof(Post)).ShouldNotBeNull();
        }

        [TestMethod]
        public void WithSimpleProperty_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            var post = new Post() { Title = "test" };

            //Act
            builder
                .Resource<Post>()
                .WithSimpleProperty(p => p.Title);

            var configuration = builder.Build();
            var mapping = configuration.GetMapping(typeof(Post));

            //Assert
            mapping.PropertyGetters.Count.ShouldEqual(1);
            mapping.PropertySetters.Count.ShouldEqual(1);

            var getter = mapping.PropertyGetters.Single().Value;
            var setter = mapping.PropertySetters.Single().Value;

            ((string)getter(post)).ShouldEqual("test");

            setter(post, "works");
            post.Title.ShouldEqual("works");
        }

        [TestMethod]
        public void WithIdSelector_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            var post = new Post { Id = 4 };

            //Act
            builder
                .Resource<Post>()
                .WithIdSelector(p => p.Id);

            var configuration = builder.Build();
            var mapping = configuration.GetMapping(typeof(Post));

            //Assert
            mapping.IdGetter.ShouldNotBeNull();
            mapping.IdGetter(post).ShouldEqual(4);
        }

        [TestMethod]
        public void WithLinkedResource_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            builder
                .WithConvention(new CamelCaseLinkNameConvention())
                .WithConvention(new PluralizedCamelCaseTypeConvention())
                .WithConvention(new SimpleLinkedIdConvention());

            var post = new Post();
            var author = new Author
            {
                Posts = new List<Post> { post }
            };

            post.Author = author;
            post.AuthorId = 4;

            //Act
            builder
                .Resource<Post>()
                .WithLinkedResource(p => p.Author);

            builder
                .Resource<Author>()
                .WithLinkedResource(a => a.Posts);

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));
            var authorMapping = configuration.GetMapping(typeof(Author));

            //Assert
            postMapping.Links.Count.ShouldEqual(1);

            var linkToAuthor = postMapping.Links.Single();

            linkToAuthor.IsCollection.ShouldBeFalse();
            linkToAuthor.LinkName.ShouldEqual("author");
            linkToAuthor.ParentType.ShouldEqual(typeof(Post));
            linkToAuthor.LinkedType.ShouldEqual(typeof(Author));
            linkToAuthor.Resource(post).ShouldBeSameAs(author);
            linkToAuthor.ResourceId(post).ShouldEqual(4);
            linkToAuthor.ResourceMapping.ShouldBeSameAs(authorMapping);

            authorMapping.Links.Count.ShouldEqual(1);
            var linkToPosts = authorMapping.Links.Single();

            linkToPosts.IsCollection.ShouldBeTrue();
            linkToPosts.LinkName.ShouldEqual("posts");
            linkToPosts.LinkedType.ShouldEqual(typeof(Post));
            linkToPosts.ParentType.ShouldEqual(typeof(Author));
            linkToPosts.Resource(author).ShouldBeSameAs(author.Posts);
            linkToPosts.ResourceId.ShouldBeNull();
            linkToPosts.ResourceMapping.ShouldBeSameAs(postMapping);
        }

        [TestMethod]
        public void WithLinkedResource_uses_conventions()
        {
            //Arrange
            const string testResourceType = "testResourceType";
            const string testLinkName = "testLinkName";
            Expression<Func<Post, object>> testIdExpression = p => 4;

            var builder = new ConfigurationBuilder();

            var linkNameConventionMock = A.Fake<ILinkNameConvention>();
            A.CallTo(() => linkNameConventionMock.GetLinkNameFromExpression(A<Expression<Func<Post, Author>>>._)).Returns(testLinkName);

            var resourceTypeMock = A.Fake<IResourceTypeConvention>();
            A.CallTo(() => resourceTypeMock.GetResourceTypeFromRepresentationType(A<Type>._)).Returns(testResourceType);

            var linkedIdConventionMock = A.Fake<ILinkIdConvention>();
            A.CallTo(() => linkedIdConventionMock.GetIdExpression(A<Expression<Func<Post, Author>>>._)).Returns(testIdExpression);

            
            builder
                .WithConvention(linkNameConventionMock)
                .WithConvention(resourceTypeMock)
                .WithConvention(linkedIdConventionMock);
            
            //Act
            builder
                .Resource<Post>()
                .WithLinkedResource(p => p.Author);

            builder
                .Resource<Author>()
                .WithResourceType(testResourceType);

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));
            var link = postMapping.Links.Single();

            //Assert
            link.LinkName.ShouldEqual(testLinkName);
            link.LinkedResourceType.ShouldEqual(testResourceType);
            link.ResourceId(new Post()).ShouldEqual(4);
        }

        [TestMethod]
        public void WithAllSimpleProperties_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            builder
                .WithConvention(new DefaultPropertyScanningConvention());

            const string testTitle = "test";
            var post = new Post
            {
                Id = 4,
                Title = testTitle
            };

            //Act
            builder
                .Resource<Post>()
                .WithAllSimpleProperties();
            
            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));

            //Assert
            postMapping.IdGetter.ShouldNotBeNull();
            postMapping.IdGetter(post).ShouldEqual(4);
            postMapping.PropertyGetters.Count.ShouldEqual(2);
            postMapping.PropertySetters.Count.ShouldEqual(2);
            postMapping.PropertyGetters["title"](post).ShouldEqual(testTitle);
            postMapping.PropertyGetters.ContainsKey("authorId").ShouldBeTrue();
        }

        [TestMethod]
        public void WithAllLinkedResources_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            builder
                .WithConvention(new DefaultPropertyScanningConvention())
                .WithConvention(new CamelCaseLinkNameConvention())
                .WithConvention(new PluralizedCamelCaseTypeConvention())
                .WithConvention(new SimpleLinkedIdConvention());
            
            //Act
            builder
                .Resource<Post>()
                .WithAllLinkedResources();

            builder.Resource<Author>();
            builder.Resource<Comment>();

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));

            //Assert
            postMapping.Links.Count.ShouldEqual(2);
            postMapping.Links.SingleOrDefault(l => l.LinkedResourceType == "authors").ShouldNotBeNull();
            postMapping.Links.SingleOrDefault(l => l.LinkedResourceType == "comments").ShouldNotBeNull();
        }

        [TestMethod]
        public void WithAllProperties_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            builder
                .WithConvention(new DefaultPropertyScanningConvention())
                .WithConvention(new CamelCaseLinkNameConvention())
                .WithConvention(new PluralizedCamelCaseTypeConvention())
                .WithConvention(new SimpleLinkedIdConvention());

            //Act
            builder
                .Resource<Post>()
                .WithAllProperties();

            builder.Resource<Author>();
            builder.Resource<Comment>();

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));

            //Assert
            postMapping.Links.Count.ShouldEqual(2);
            postMapping.Links.SingleOrDefault(l => l.LinkedResourceType == "authors").ShouldNotBeNull();
            postMapping.Links.SingleOrDefault(l => l.LinkedResourceType == "comments").ShouldNotBeNull();
            postMapping.PropertyGetters.Count.ShouldEqual(2);
            postMapping.PropertySetters.Count.ShouldEqual(2);
            postMapping.IdGetter.ShouldNotBeNull();
        }

        [TestMethod]
        public void WithAllProperties_uses_conventions()
        {
            //Arrange
            const string testResourceType = "testResourceType";
            const string testLinkName = "testLinkName";
            string testname = "testName";

            Expression<Func<Post, object>> testIdExpression = p => 4;

            var builder = new ConfigurationBuilder();

            var linkNameConventionMock = A.Fake<ILinkNameConvention>();
            A.CallTo(() => linkNameConventionMock.GetLinkNameFromExpression(A<Expression<Func<Post, Author>>>._)).Returns(testLinkName);

            var resourceTypeMock = A.Fake<IResourceTypeConvention>();
            A.CallTo(() => resourceTypeMock.GetResourceTypeFromRepresentationType(A<Type>._)).Returns(testResourceType);

            var linkedIdConventionMock = A.Fake<ILinkIdConvention>();
            A.CallTo(() => linkedIdConventionMock.GetIdExpression(A<Expression<Func<Post, Author>>>._)).Returns(testIdExpression);

            var propertyScanningConventionMock = A.Fake<IPropertyScanningConvention>();
            A
                .CallTo(() => propertyScanningConventionMock.IsLinkedResource(A<PropertyInfo>
                    .That.Matches(pi => pi.Name == "Replies" || pi.Name == "Author")))
                .Returns(true);

            A.CallTo(() => propertyScanningConventionMock.IsPrimaryId(A<PropertyInfo>.That.Matches(pi => pi.Name == "Id"))).Returns(true);

            A.CallTo(() => propertyScanningConventionMock.GetPropertyName(A<PropertyInfo>.That.Matches(pi => pi.Name == "Title"))).Returns(testname);
            A.CallTo(() => propertyScanningConventionMock.GetPropertyName(A<PropertyInfo>.That.Matches(pi => pi.Name == "AuthorId"))).Returns("authorId");

            builder
                .WithConvention(propertyScanningConventionMock)
                .WithConvention(linkNameConventionMock)
                .WithConvention(resourceTypeMock)
                .WithConvention(linkedIdConventionMock);

            //Act
            builder
                .Resource<Post>()
                .WithAllProperties();

            builder
                .Resource<Author>()
                .WithResourceType(testResourceType);

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));
            var link = postMapping.Links.Single();

            //Assert
            link.LinkName.ShouldEqual(testLinkName);
            link.LinkedResourceType.ShouldEqual(testResourceType);
            link.ResourceId(new Post()).ShouldEqual(4);
            postMapping.PropertyGetters.ContainsKey(testname).ShouldBeTrue();

            A.CallTo(() => propertyScanningConventionMock.IsLinkedResource(A<PropertyInfo>._)).MustHaveHappened();
            A.CallTo(() => propertyScanningConventionMock.IsPrimaryId(A<PropertyInfo>._)).MustHaveHappened();
            A.CallTo(() => propertyScanningConventionMock.ShouldIgnore(A<PropertyInfo>._)).MustHaveHappened();
            A.CallTo(() => propertyScanningConventionMock.ThrowOnUnmappedLinkedType).MustHaveHappened();
        }

        [TestMethod]
        public void WithComplexObjectTest()
        {
            //Arrange
            const int authorId = 5;
            const string authorName = "Valentin";
            const int postId = 6;
            const string postTitle = "The measure of a man";
            const string commentBody = "Comment body";
            const int commentId = 7;
            var author = new Author() { Id = authorId, Name = authorName };
            var post = new Post() { Id = postId, Title = postTitle, Author = author };
            var comment = new Comment() { Id = commentId, Body = commentBody, Post = post };
            post.Replies = new List<Comment>() { comment };
            author.Posts = new List<Post>() { post };

            var configurationBuilder = new ConfigurationBuilder();

            //Act
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithSimpleProperty(p => p.Title)
                .WithIdSelector(p => p.Id)
                .WithLinkedResource(p => p.Replies);
            var resourceConfigurationForAuthor = configurationBuilder
                .Resource<Author>()
                .WithSimpleProperty(a => a.Name)
                .WithIdSelector(a => a.Id)
                .WithLinkedResource(a => a.Posts);
            var resourceConfigurationForComment = configurationBuilder
                .Resource<Comment>()
                .WithIdSelector(c => c.Id)
                .WithSimpleProperty(c => c.Body);
            var result = configurationBuilder.Build();

            //Assert
            resourceConfigurationForPost.ConstructedMetadata.Links.Count.ShouldEqual(1);
            resourceConfigurationForAuthor.ConstructedMetadata.Links.Count.ShouldEqual(1);
            configurationBuilder.ResourceConfigurationsByType.All(
                r => r.Value.ConstructedMetadata.Links.All(l => l.ResourceMapping != null));
            var authorLinks =
                 configurationBuilder.ResourceConfigurationsByType[
                     resourceConfigurationForAuthor.ConstructedMetadata.ResourceRepresentationType].ConstructedMetadata.Links;
            authorLinks.ShouldNotBeNull().Count.ShouldEqual(1);
            authorLinks[0].LinkName.ShouldEqual("posts");
            authorLinks[0].ResourceMapping.PropertyGetters.ShouldNotBeNull().Count.ShouldEqual(1);
            authorLinks[0].ResourceMapping.Links
                .ForEach(p => p.ResourceMapping.Links
                    .ForEach(c => c
                        .LinkName
                        .ShouldEqual(resourceConfigurationForComment.ConstructedMetadata.ResourceType)));

        }
    }
}
