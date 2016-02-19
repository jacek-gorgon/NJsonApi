using Microsoft.AspNet.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Serialization.BadActionResultTransformers
{
    internal class TransformHttpUnauthorizedResult : BaseTransformBadAction<HttpUnauthorizedResult>
    {
        public override Error GetError(HttpUnauthorizedResult result)
        {
            return new Error()
            {
                Title = "You were not authorised.",
                Status = 403
            };
        }
    }
}
