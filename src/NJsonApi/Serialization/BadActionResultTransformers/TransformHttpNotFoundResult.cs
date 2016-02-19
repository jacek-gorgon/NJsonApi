using NJsonApi.Serialization.Representations;
using Microsoft.AspNet.Mvc;

namespace NJsonApi.Serialization.BadActionResultTransformers
{
    internal class TransformHttpNotFoundResult : BaseTransformBadAction<HttpNotFoundResult>
    {
        public override Error GetError(HttpNotFoundResult result)
        {
            return new Error()
            {
                Title = "The result was not found",
                Status = 404
            };
        }
    }
}
