using Microsoft.AspNet.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.BadActionResultTransformers
{
    internal class TransformBadRequestResult : BaseTransformBadAction<BadRequestResult>
    {
        public override Error GetError(BadRequestResult result)
        {
            return new Error()
            {
                Title = $"There was a bad request.",
                Status = 400
            };
        }
    }
}
