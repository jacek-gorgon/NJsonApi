using Microsoft.AspNet.Mvc;
using NJsonApi.Web.BadActionResultTransformers;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NJsonApi.Test.Serialization
{
    public class BadActionTransformerTests
    {

        [Fact]
        public void GIVEN_AGoodAction_WHEN_IsBadAction_THEN_False()
        {
            // Arrange
            var objectResult = new JsonResult(new Post());

            // Act
            var result = BadActionResultTransformer.IsBadAction(objectResult);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void GIVEN_ABadAction_WHEN_IsBadAction_THEN_True()
        {
            // Arrange
            var objectResult = new HttpNotFoundObjectResult("1234");

            // Act
            var result = BadActionResultTransformer.IsBadAction(objectResult);

            // Assert
            Assert.True(result);
        }


        [Fact]
        public void GIVEN_AGoodAction_WHEN_Transform_THEN_Exception()
        {
            // Arrange
            var objectResult = new ObjectResult(new Post());

            // Act - Throw
            Assert.Throws<InvalidOperationException>(() => BadActionResultTransformer.Transform(objectResult));

            // Assert
        }


        [Fact]
        public void GIVEN_ABadAction_WHEN_Transform_THEN_CompoundDocumentWithError()
        {
            // Arrange
            var objectResult = new HttpNotFoundObjectResult(123);

            // Act
            var result = BadActionResultTransformer.Transform(objectResult);

            // Assert
            Assert.NotEmpty(result.Errors);
            Assert.Equal(404, result.Errors.First().Status);
            Assert.NotEmpty(result.Errors.First().Title);
        }
    }
}
