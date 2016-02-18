using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using NJsonApi.Serialization;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations.Resources;
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
    public class JsonApiActionFilterTests
    {
        [Fact]
        public void GIVEN_PostObject_WHEN_OnActionExecuted_THEN_ResponseValid()
        {
            // Arrange
            var actionFilter = GetActionFilterForTestModel();

            var post = new PostBuilder()
                .WithAuthor(PostBuilder.Asimov)
                .Build();

            var context = new ActionExecutedContextBuilder()
                .WithResult(new ObjectResult(post))
                .Build();

            // Act
            actionFilter.OnActionExecuted(context);

            // Assert
            var result = (ObjectResult)context.Result;
            var value = (CompoundDocument)result.Value;
            var resource = (SingleResource)value.Data;

            Assert.Null(value.Errors);
            Assert.Equal(post.Title, resource.Attributes["title"]);
            Assert.Equal(post.AuthorId, resource.Attributes["authorId"]);
        }

        private JsonApiActionFilter GetActionFilterForTestModel()
        {
            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var transformer = new JsonApiTransformer();
            return new JsonApiActionFilter(transformer, config);
        }
    }
}
