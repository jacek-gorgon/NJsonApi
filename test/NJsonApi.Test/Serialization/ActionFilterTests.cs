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
    public class ActionFilterTests
    {
        [Fact]
        public void GIVEN_PostObject_WHEN_OnActionExecuted_THEN_ResponseValid()
        {
            // Arrange
            var actionFilter = GetActionFilterForTestModel();

            var post = new PostBuilder()
                .WithAuthor(PostBuilder.Asimov)
                .Build();

            var context = new FilterContextBuilder()
                .WithResult(new ObjectResult(post))
                .BuildActionExecuted();

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


        [Fact]
        public void GIVEN_Exception_WHEN_OnActionExecuted_THEN_ExceptionIsInCompoundDocument()
        {
            // Arrange
            var transformer = new JsonApiTransformer();
            var exceptionFilter = new JsonApiExceptionFilter(transformer);


            var post = new PostBuilder()
                .WithAuthor(PostBuilder.Asimov)
                .Build();

            var context = new FilterContextBuilder()
                .WithException("Test exception message")
                .BuildException();

            // Act
            exceptionFilter.OnException(context);

            // Assert
            var result = (ObjectResult)context.Result;
            var value = (CompoundDocument)result.Value;

            Assert.Equal(1, value.Errors.Count());
            Assert.Equal("Test exception message", value.Errors.First().Detail);
            Assert.Equal(500, value.Errors.First().Status);
        }

        [Fact]
        public void GIVEN_HttpNotFoundObjectResult_WHEN_ActionExecuted_THEN_ErrorValue()
        {
            // Arrange
            var actionFilter = GetActionFilterForTestModel();

            var idNotFoundResult = new HttpNotFoundObjectResult(42);

            var context = new FilterContextBuilder()
                .WithResult(idNotFoundResult)
                .BuildActionExecuted();

            // Act
            actionFilter.OnActionExecuted(context);

            // Assert
            var result = (ObjectResult)context.Result;
            var value = (CompoundDocument)result.Value;

            Assert.Equal(1, value.Errors.Count());
        }


        [Fact]
        public void GIVEN_WrongMediaType_WHEN_ActionExecuting_THEN_ResponseSet()
        {
            // Arrange
            string mediaType = null;

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithMediaType(mediaType)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            var result = (UnsupportedMediaTypeResult)context.Result;
            Assert.Equal(415, result.StatusCode);
        }

        [Fact]
        public void GIVEN_CorrectMediaType_WHEN_ActionExecuting_THEN_ResponseNotSet()
        {
            // Arrange
            string mediaType = "application/vnd.api+json";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithMediaType(mediaType)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }


        private JsonApiActionFilter GetActionFilterForTestModel()
        {
            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var transformer = new JsonApiTransformer();
            return new JsonApiActionFilter(transformer, config);
        }

    }
}
