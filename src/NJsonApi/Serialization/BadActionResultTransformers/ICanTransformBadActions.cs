using Microsoft.AspNet.Mvc;
using NJsonApi.Serialization.Documents;

namespace NJsonApi.Serialization.BadActionResultTransformers
{
    internal interface ICanTransformBadActions
    {
        bool Accepts(IActionResult result);

        CompoundDocument Transform(IActionResult result);
    }
}
