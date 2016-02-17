using NJsonApi;
using NJsonApi.Serialization;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NJsonApi.Test.Serialization
{
    public class IncludedResourcesTests
    {
        [Fact]
        public void AppendIncludedRepresentationRecursive_RecursesWholeTree()
        {
            // Arrange
            var source = new PostBuilder()
                .WithAuthor(PostBuilder.Asimov)
                .WithComment(1, "Comment One")
                .WithComment(2, "Comment Two")
                .Build();

            var sourceList = new List<Post>()
            {
                source
            };

            var result = new List<SingleResource>();
            var alreadyVisitedObjects = new HashSet<object>(sourceList);

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();

            var mapping = config.GetMapping(typeof(Post));
            var context = new Context(config, new Uri("http://dummy:4242/posts"));

            var transformationHelper = new TransformationHelper();

            // Act
            result = transformationHelper.AppendIncludedRepresentationRecursive(source, mapping, alreadyVisitedObjects, context);

            // Assert
            Assert.NotNull(result.Single(x => x.Id == "1" && x.Type == "comments"));
            Assert.NotNull(result.Single(x => x.Id == "2" && x.Type == "comments"));
            Assert.NotNull(result.Single(x => x.Id == "1" && x.Type == "authors"));
            Assert.False(result.Any(x => x.Type == "posts"));
        }

        [Fact]
        public void AppendIncludedRepresentationRecursive_RecursesWholeTree_No_Duplicates()
        {
            // Arrange
            var duplicateAuthor = PostBuilder.Asimov;

            var firstSource = new PostBuilder()
                .WithAuthor(duplicateAuthor)
                .Build();

            var secondSource = new PostBuilder()
                .WithAuthor(duplicateAuthor)
                .Build();

            var sourceList = new List<Post>()
            {
                firstSource,
                secondSource
            };

            var result = new List<SingleResource>();
            var alreadyVisitedObjects = new HashSet<object>(sourceList);

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();

            var mapping = config.GetMapping(typeof(Post));
            var context = new Context(config, new Uri("http://dummy:4242/posts"));

            var transformationHelper = new TransformationHelper();

            // Act
            foreach (var source in sourceList)
            {
                result.AddRange(transformationHelper.AppendIncludedRepresentationRecursive(source, mapping, alreadyVisitedObjects, context));
            }

            // Assert
            Assert.Equal(1, result.Count(x => 
                x.Type == "authors" && 
                x.Id == PostBuilder.Asimov.Id.ToString()));
        }
    }
}
