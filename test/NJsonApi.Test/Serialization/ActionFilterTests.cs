using Microsoft.AspNet.Mvc;
using NJsonApi.Serialization;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.Builders;
using System.Linq;
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
        public void GIVEN_WrongContentType_WHEN_ActionExecuting_THEN_ResponseIs415()
        {
            // Arrange
            string contentType = null;

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            var result = (UnsupportedMediaTypeResult)context.Result;
            Assert.Equal(415, result.StatusCode);
        }

        [Fact]
        public void GIVEN_CorrectContentType_WHEN_ActionExecuting_THEN_ResponseNotSet()
        {
            // Arrange
            string contentType = "application/vnd.api+json";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }


        [Fact]
        public void GIVEN_CorrectContentType_AND_Parameters_WHEN_ActionExecuting_THEN_ResponseIs415()
        {
            // Arrange
            string contentType = "application/vnd.api+json; version=1.0";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            var result = (UnsupportedMediaTypeResult)context.Result;
            Assert.Equal(415, result.StatusCode);
        }


        [Fact]
        public void GIVEN_IncorrectAcceptsHeader_WHEN_ActionExecuting_THEN_ResponseIs406()
        {
            // Arrange
            string acceptsHeader = "application/vnd.api+json; version=1.0";
            string contentType = "application/vnd.api+json";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .WithHeader("Accept", acceptsHeader)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            var result = (HttpStatusCodeResult)context.Result;
            Assert.Equal(406, result.StatusCode);
        }

        [Fact]
        public void GIVEN_CorrectAcceptsHeader_WHEN_ActionExecuting_THEN_ResponseIsNull()
        {
            // Arrange
            string acceptsHeader = "application/vnd.api+json";
            string contentType = "application/vnd.api+json";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .WithHeader("Accept", acceptsHeader)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }


        [Fact]
        public void GIVEN_MutlipleAccept_AND_Correct_WHEN_ActionExecuting_THEN_ResponseIsNull()
        {
            // Arrange
            string acceptsHeader = "application/vnd.api+json, application/vnd.api+json; version=1.0, application/json";
            string contentType = "application/vnd.api+json";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .WithHeader("Accept", acceptsHeader)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }

        [Fact]
        public void GIVEN_MutlipleAccept_AND_AllAreWrong_WHEN_ActionExecuting_THEN_406Unacceptable()
        {
            // Arrange
            string acceptsHeader = "application/xml, application/vnd.api+json; version=1.0, application/json";
            string contentType = "application/vnd.api+json";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .WithHeader("Accept", acceptsHeader)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            var result = (HttpStatusCodeResult)context.Result;
            Assert.Equal(406, result.StatusCode);
        }

        [Fact]
        public void GIVEN_WildcardAccept_WHEN_ActionExecuting_THEN_ResponseIsNull()
        {
            // Arrange
            string acceptsHeader = "*/*";
            string contentType = "application/vnd.api+json";

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithContentType(contentType)
                .WithHeader("Accept", acceptsHeader)
                .BuildActionExecuting();

            // Act
            actionFilter.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }

        [Theory]
        [InlineData("singlenonsensepath")]
        [InlineData("double.nonsensepath")]
        [InlineData("post.nonsense")]
        [InlineData("nonsense.comment")]
        public void GIVEN_IncludesAreNotApplicable_WHEN_ActionExecuted_THEN_400BadRequest(string includePath)
        {
            // Arrange
            var post = new PostBuilder()
                .WithAuthor(PostBuilder.Asimov)
                .Build();

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithResult(new ObjectResult(post))
                .WithQuery("include", includePath)
                .BuildActionExecuted();

            // Act
            actionFilter.OnActionExecuted(context);

            // Assert
            var result = (HttpStatusCodeResult)context.Result;
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public void GIVEN_SingleInclude_WHEN_ActionExecuted_THEN_RelatedIncludePresent()
        {
            // Arrange
            var post = new PostBuilder()
                .WithAuthor(PostBuilder.Asimov)
                .Build();

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithResult(new ObjectResult(post))
                .WithQuery("include", "authors")
                .BuildActionExecuted();

            // Act
            actionFilter.OnActionExecuted(context);

            // Assert
            var result = (ObjectResult)context.Result;
            var document = (CompoundDocument)result.Value;

            Assert.Equal(PostBuilder.Asimov.Name, document.Included.Single().Attributes["name"]);
        }

        [Fact]
        public void GIVEN_MultipleInclude_WHEN_ActionExecuted_THEN_AllIncludesPresent()
        {
            // Arrange
            var post = new PostBuilder()
                .WithAuthor(PostBuilder.Asimov)
                .WithComment(1, "First")
                .Build();

            var actionFilter = GetActionFilterForTestModel();
            var context = new FilterContextBuilder()
                .WithResult(new ObjectResult(post))
                .WithQuery("include", "authors,comments")
                .BuildActionExecuted();

            // Act
            actionFilter.OnActionExecuted(context);

            // Assert
            var result = (ObjectResult)context.Result;
            var document = (CompoundDocument)result.Value;
            var includedAttributes = document.Included.SelectMany(x => x.Attributes).ToList();

            Assert.Contains(includedAttributes, x =>
                x.Key == "name" &&
                x.Value.ToString() == PostBuilder.Asimov.Name);

            Assert.Contains(includedAttributes, x =>
                x.Key == "body" &&
                x.Value.ToString() == "First");
        }

        private JsonApiActionFilter GetActionFilterForTestModel()
        {
            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var transformer = new JsonApiTransformer();
            return new JsonApiActionFilter(transformer, config);
        }
    }
}
