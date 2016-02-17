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

            var config = TestModelConfigurationBuilder.BuildEverything;

            var mapping = config.GetMapping(typeof(Post));
            var context = new Context(config, new Uri("http://dummy:4242/posts"));

            var transformationHelper = new TransformationHelper();

            // Act
            transformationHelper.AppendIncludedRepresentationRecursive(source, mapping, result, alreadyVisitedObjects, context);

            // Assert
            Assert.NotNull(result.Single(x => x.Id == "1" && x.Type == "comments"));
            Assert.NotNull(result.Single(x => x.Id == "2" && x.Type == "comments"));
            Assert.NotNull(result.Single(x => x.Id == "1" && x.Type == "authors"));
            Assert.False(result.Any(x => x.Type == "posts"));
        }
    }
}
