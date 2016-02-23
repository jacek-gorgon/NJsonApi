using Microsoft.AspNet.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.BadActionResultTransformers
{
    internal class TransformHttpNotFoundObjectResult : BaseTransformBadAction<HttpNotFoundObjectResult>
    {
        public override Error GetError(HttpNotFoundObjectResult result)
        {
            return new Error()
            {
                Title = $"The result with id {result.Value} was not found",
                Status = result.StatusCode.Value
            };
        }
    }
}
