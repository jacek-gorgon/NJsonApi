using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NJsonApi.Test.TestModel;
using NJsonApi.Conventions;
using NJsonApi.Conventions.Impl;
using Xunit;

namespace NJsonApi.Test
{
    public class ConfigurationBuilderTest
    {
        [Fact]
        public void Resource_creates_mapping()
        {
            //Arrange
            var builder = new ConfigurationBuilder();

            //Act
            builder.Resource<Post>();

            var result = builder.Build();

            //Assert
            Assert.True(result.IsMappingRegistered(typeof(Post)));
            Assert.NotNull(result.GetMapping(typeof(Post)));
        }

        [Fact]
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
            Assert.Equal(mapping.PropertyGetters.Count, 1);
            Assert.Equal(mapping.PropertySetters.Count, 1);

            var getter = mapping.PropertyGetters.Single().Value;
            var setter = mapping.PropertySetters.Single().Value;

            Assert.Equal(((string)getter(post)), "test");

            setter(post, "works");
            Assert.Equal(post.Title, "works");
        }

        [Fact]
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
            Assert.NotNull(mapping.IdGetter);
            Assert.Equal(mapping.IdGetter(post), 4);
        }

        [Fact]
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
            Assert.Equal(postMapping.Relationships.Count, 1);

            var linkToAuthor = postMapping.Relationships.Single();

            Assert.False(linkToAuthor.IsCollection);
            Assert.Equal(linkToAuthor.RelationshipName, "author");
            Assert.Equal(linkToAuthor.ParentType, typeof(Post));
            Assert.Equal(linkToAuthor.RelatedBaseType, typeof(Author));
            Assert.Same(linkToAuthor.RelatedResource(post), author);
            Assert.Equal(linkToAuthor.RelatedResourceId(post), 4);
            Assert.Same(linkToAuthor.ResourceMapping, authorMapping);
            Assert.Equal(authorMapping.Relationships.Count, 1);
            
            var linkToPosts = authorMapping.Relationships.Single();

            Assert.True(linkToPosts.IsCollection);
            Assert.Equal(linkToPosts.RelationshipName, "posts");
            Assert.Equal(linkToPosts.ParentType, typeof(Author));
            Assert.Equal(linkToPosts.RelatedBaseType, typeof(Post));
            Assert.Same(linkToPosts.RelatedResource(author), author.Posts);
            Assert.Null(linkToPosts.RelatedResourceId);
            Assert.Same(linkToPosts.ResourceMapping, postMapping);
            
        }

        // TODO - Mocking framework not currently available for .NET Core
        // Replace this test when there is one
        //[TestMethod]
        //public void WithLinkedResource_uses_conventions()
        //{
        //    //Arrange
        //    const string testResourceType = "testResourceType";
        //    const string testLinkName = "testLinkName";
        //    Expression<Func<Post, object>> testIdExpression = p => 4;

        //    var builder = new ConfigurationBuilder();

        //    var linkNameConventionMock = A.Fake<ILinkNameConvention>();
        //    A.CallTo(() => linkNameConventionMock.GetLinkNameFromExpression(A<Expression<Func<Post, Author>>>._)).Returns(testLinkName);

        //    var resourceTypeMock = A.Fake<IResourceTypeConvention>();
        //    A.CallTo(() => resourceTypeMock.GetResourceTypeFromRepresentationType(A<Type>._)).Returns(testResourceType);

        //    var linkedIdConventionMock = A.Fake<ILinkIdConvention>();
        //    A.CallTo(() => linkedIdConventionMock.GetIdExpression(A<Expression<Func<Post, Author>>>._)).Returns(testIdExpression);

            
        //    builder
        //        .WithConvention(linkNameConventionMock)
        //        .WithConvention(resourceTypeMock)
        //        .WithConvention(linkedIdConventionMock);
            
        //    //Act
        //    builder
        //        .Resource<Post>()
        //        .WithLinkedResource(p => p.Author);

        //    builder
        //        .Resource<Author>()
        //        .WithResourceType(testResourceType);

        //    var configuration = builder.Build();
        //    var postMapping = configuration.GetMapping(typeof(Post));
        //    var link = postMapping.Relationships.Single();

        //    //Assert
        //    link.RelationshipName.ShouldEqual(testLinkName);
        //    link.RelatedBaseResourceType.ShouldEqual(testResourceType);
        //    link.RelatedResourceId(new Post()).ShouldEqual(4);
        //}

        [Fact]
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
            Assert.NotNull(postMapping.IdGetter);
            Assert.Equal(postMapping.IdGetter(post), 4);
            Assert.Equal(postMapping.PropertyGetters.Count, 2);
            Assert.Equal(postMapping.PropertySetters.Count, 2);
            Assert.Equal(postMapping.PropertyGetters["title"](post), testTitle);
            Assert.True(postMapping.PropertyGetters.ContainsKey("authorId"));
        }

        [Fact]
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
            Assert.Equal(postMapping.Relationships.Count, 2);
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "authors"));
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "comments"));
        }

        [Fact]
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
            Assert.Equal(postMapping.Relationships.Count, 2);
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "authors"));
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "comments"));
            Assert.Equal(postMapping.PropertyGetters.Count, 2);
            Assert.Equal(postMapping.PropertySetters.Count, 2);
            Assert.NotNull(postMapping.IdGetter);
        }

        // TODO - Mocking framework not currently available for .NET Core
        // Replace this test when there is one
        //[TestMethod]
        //public void WithAllProperties_uses_conventions()
        //{
        //    //Arrange
        //    const string testResourceType = "testResourceType";
        //    const string testLinkName = "testLinkName";
        //    string testname = "testName";

        //    Expression<Func<Post, object>> testIdExpression = p => 4;

        //    var builder = new ConfigurationBuilder();

        //    var linkNameConventionMock = A.Fake<ILinkNameConvention>();
        //    A.CallTo(() => linkNameConventionMock.GetLinkNameFromExpression(A<Expression<Func<Post, Author>>>._)).Returns(testLinkName);

        //    var resourceTypeMock = A.Fake<IResourceTypeConvention>();
        //    A.CallTo(() => resourceTypeMock.GetResourceTypeFromRepresentationType(A<Type>._)).Returns(testResourceType);

        //    var linkedIdConventionMock = A.Fake<ILinkIdConvention>();
        //    A.CallTo(() => linkedIdConventionMock.GetIdExpression(A<Expression<Func<Post, Author>>>._)).Returns(testIdExpression);

        //    var propertyScanningConventionMock = A.Fake<IPropertyScanningConvention>();
        //    A
        //        .CallTo(() => propertyScanningConventionMock.IsLinkedResource(A<PropertyInfo>
        //            .That.Matches(pi => pi.Name == "Replies" || pi.Name == "Author")))
        //        .Returns(true);

        //    A.CallTo(() => propertyScanningConventionMock.IsPrimaryId(A<PropertyInfo>.That.Matches(pi => pi.Name == "Id"))).Returns(true);

        //    A.CallTo(() => propertyScanningConventionMock.GetPropertyName(A<PropertyInfo>.That.Matches(pi => pi.Name == "Title"))).Returns(testname);
        //    A.CallTo(() => propertyScanningConventionMock.GetPropertyName(A<PropertyInfo>.That.Matches(pi => pi.Name == "AuthorId"))).Returns("authorId");

        //    builder
        //        .WithConvention(propertyScanningConventionMock)
        //        .WithConvention(linkNameConventionMock)
        //        .WithConvention(resourceTypeMock)
        //        .WithConvention(linkedIdConventionMock);

        //    //Act
        //    builder
        //        .Resource<Post>()
        //        .WithAllProperties();

        //    builder
        //        .Resource<Author>()
        //        .WithResourceType(testResourceType);

        //    var configuration = builder.Build();
        //    var postMapping = configuration.GetMapping(typeof(Post));
        //    var link = postMapping.Relationships.Single();

        //    //Assert
        //    link.RelationshipName.ShouldEqual(testLinkName);
        //    link.RelatedBaseResourceType.ShouldEqual(testResourceType);
        //    link.RelatedResourceId(new Post()).ShouldEqual(4);
        //    postMapping.PropertyGetters.ContainsKey(testname).ShouldBeTrue();

        //    A.CallTo(() => propertyScanningConventionMock.IsLinkedResource(A<PropertyInfo>._)).MustHaveHappened();
        //    A.CallTo(() => propertyScanningConventionMock.IsPrimaryId(A<PropertyInfo>._)).MustHaveHappened();
        //    A.CallTo(() => propertyScanningConventionMock.ShouldIgnore(A<PropertyInfo>._)).MustHaveHappened();
        //    A.CallTo(() => propertyScanningConventionMock.ThrowOnUnmappedLinkedType).MustHaveHappened();
        //}

        [Fact]
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
            Assert.Equal(resourceConfigurationForPost.ConstructedMetadata.Relationships.Count, 1);
            Assert.Equal(resourceConfigurationForAuthor.ConstructedMetadata.Relationships.Count, 1);
            configurationBuilder.ResourceConfigurationsByType.All(
                r => r.Value.ConstructedMetadata.Relationships.All(l => l.ResourceMapping != null));
            var authorLinks =
                 configurationBuilder.ResourceConfigurationsByType[
                     resourceConfigurationForAuthor.ConstructedMetadata.ResourceRepresentationType].ConstructedMetadata.Relationships;
            Assert.Equal(authorLinks.Count, 1);
            Assert.Equal(authorLinks[0].RelationshipName, "posts");
            Assert.Equal(authorLinks[0].ResourceMapping.PropertyGetters.Count, 1);
        }
    }
}
