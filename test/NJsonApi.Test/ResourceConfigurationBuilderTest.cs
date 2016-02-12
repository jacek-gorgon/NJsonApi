using NJsonApi.Test.TestModel;
using Xunit;

namespace NJsonApi.Test
{
    public class ResourceConfigurationBuilderTest
    {
        private ConfigurationBuilder configurationBuilder;

        public ResourceConfigurationBuilderTest()
        {
            configurationBuilder = new ConfigurationBuilder();
        }

        [Fact]
        public void TestWithResourceType()
        {
            //Arrange
            string resourceType = typeof(Author).Name;
            var classUnderTest = configurationBuilder.Resource<Author>();

            //Act
            classUnderTest.WithResourceType(resourceType);

            //Assert
            Assert.Equal(classUnderTest.ConstructedMetadata.ResourceType, resourceType);
        }

        [Fact]
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
            Assert.Equal(resourceConfigurationForAuthor
                .ConstructedMetadata
                .ResourceType, resourceTypeAuthor);

            Assert.Equal(resourceConfigurationForPost
                .ConstructedMetadata
                .ResourceType,resourceTypePost);
        }

        [Fact]
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
            Assert.Equal(result, authorId);
        }

        [Fact]
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
            Assert.Equal(authorResult, authorId);
            var postResult = (int)resourceConfigurationForPost.ConstructedMetadata.IdGetter.Invoke(post);
            Assert.Equal(postResult, postId);
        }

        [Fact]
        public void TestWithSimpleProperty()
        {
            //Arrange
            const int authorId = 5;
            var author = new Author() { Id = authorId };
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            
            //Act
            resourceConfigurationForAuthor.WithSimpleProperty(a => a.Name);

            //Assert
            Assert.Equal(resourceConfigurationForAuthor.ConstructedMetadata.PropertyGetters.Count, 1);
            Assert.Equal(resourceConfigurationForAuthor.ConstructedMetadata.PropertySetters.Count, 1);
            Assert.Null(resourceConfigurationForAuthor.ConstructedMetadata.IdGetter);
            Assert.Contains(resourceConfigurationForAuthor.ConstructedMetadata.ResourceType, "author");
        }

        class ClassWithReserveredKeys
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Links { get; set; }
            public string Href { get; set; }
        }

        [Fact]
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
            Assert.Contains("_id", propertyGetters.Keys);
            Assert.Contains("_type", propertyGetters.Keys);
            Assert.Contains("_links", propertyGetters.Keys);
            Assert.Contains("_href", propertyGetters.Keys);

            Assert.DoesNotContain("id", propertyGetters.Keys);
            Assert.DoesNotContain("type", propertyGetters.Keys);
            Assert.DoesNotContain("links", propertyGetters.Keys);
            Assert.DoesNotContain("href", propertyGetters.Keys);

            var propertySetters = sut.ConstructedMetadata.PropertySetters;
            Assert.Contains("_id", propertySetters.Keys);
            Assert.Contains("_type", propertySetters.Keys);
            Assert.Contains("_links", propertySetters.Keys);
            Assert.Contains("_href", propertySetters.Keys);

            Assert.DoesNotContain("id", propertySetters.Keys);
            Assert.DoesNotContain("type", propertySetters.Keys);
            Assert.DoesNotContain("links", propertySetters.Keys);
            Assert.DoesNotContain("href", propertySetters.Keys);
        }

        [Fact]
        public void TestWithSimplePropertyWithIdentity()
        {
            //Arrange & Act
            var resourceConfigurationForAuthor = configurationBuilder
                .Resource<Author>()
                .WithSimpleProperty(a => a.Name)
                .WithIdSelector(a => a.Id);
            //Assert
            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForAuthor);
            Assert.Contains("author", resourceConfigurationForAuthor.ConstructedMetadata.ResourceType);
        }

        [Fact]
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
            Assert.Equal(resultForName, authorName);
            Assert.Equal(resultForId, authorId);
        }

        [Fact]
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
            var result = resourceConfigurationForAuthor.ConstructedMetadata;
            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForAuthor);

            Assert.Contains("author", result.ResourceType);
            Assert.Equal(result.PropertyGetters["name"].Invoke(author), authorName);
            Assert.Equal(result.IdGetter.Invoke(author), authorId);

            AssertResourceConfigurationHasValuesForWithSimpleProperty(resourceConfigurationForPost);

            result = resourceConfigurationForPost.ConstructedMetadata;
            Assert.Contains(result.ResourceType, "post");
            Assert.Equal(result.PropertyGetters["title"].Invoke(post), postTitle);

            resourceConfigurationForPost.ConstructedMetadata.PropertySetters["title"].Invoke(post, postTitleModifed);
            Assert.Equal(post.Title,postTitleModifed);
            Assert.Equal(result.PropertyGetters["title"].Invoke(post), postTitleModifed);
        }

        [Fact]
        public void IgnorePropertyTest()
        {
            //Arrange
            const int authorId = 5;
            var author = new Author() { Id = authorId };
            var resourceConfigurationForAuthor = configurationBuilder.Resource<Author>();
            resourceConfigurationForAuthor.WithSimpleProperty(a => a.Name);

            var result = resourceConfigurationForAuthor.ConstructedMetadata;

            // Assert initial
            Assert.Equal(result.PropertyGetters.Count, 1);
            Assert.Equal(result.PropertySetters.Count, 1);
            Assert.Null(result.IdGetter);
            Assert.Contains("author", resourceConfigurationForAuthor.ConstructedMetadata.ResourceType);

            //Act
            resourceConfigurationForAuthor.IgnoreProperty(a => a.Name);

            //Assert
            Assert.Equal(result.PropertyGetters.Count, 0);
            Assert.Equal(result.PropertySetters.Count, 0);
            Assert.Null(result.IdGetter);
            Assert.Contains("author", resourceConfigurationForAuthor.ConstructedMetadata.ResourceType);
        }

        [Fact]
        public void WithLinkedResourceTest()
        {
            //Arrange

            //Act
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithIdSelector(p => p.Id)
                .WithSimpleProperty(p => p.Title)
                .WithLinkedResource<Author>(p => p.Author);

            var result = resourceConfigurationForPost.ConstructedMetadata;

            //Assert
            Assert.Equal(result.Relationships.Count, 1);
            Assert.Contains("author", result.Relationships[0].RelationshipName);
            Assert.Equal(result.Relationships[0].RelatedBaseType, typeof(Author));
            Assert.Null(result.Relationships[0].ResourceMapping);
        }

        [Fact]
        public void WithLinkTemplateTest()
        {
            //Arrange
            const string urlTemplate = "urlTemplate";
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post>()
                .WithIdSelector(p => p.Id)
                .WithSimpleProperty(p => p.Title);

            Assert.Null(resourceConfigurationForPost.ConstructedMetadata.UrlTemplate);

            //Act
            resourceConfigurationForPost
                 .WithLinkTemplate(urlTemplate);

            //Assert
            Assert.Equal(resourceConfigurationForPost.ConstructedMetadata.UrlTemplate,urlTemplate);
        }

        private void AssertResourceConfigurationHasValuesForWithSimpleProperty(IResourceConfigurationBuilder resourceConfiguration)
        {
            var result = resourceConfiguration.ConstructedMetadata;

            Assert.Equal(result.PropertyGetters.Count,1);
            Assert.Equal(result.PropertySetters.Count,1);
            Assert.NotNull(result.IdGetter);
        }
    }
}
