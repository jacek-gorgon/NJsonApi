using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Serialization
{
    internal class JsonApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly JsonApiTransformer jsonApiTransformer;

        public JsonApiExceptionFilter(JsonApiTransformer jsonApiTransformer)
        {
            this.jsonApiTransformer = jsonApiTransformer;
        }

        public override void OnException(ExceptionContext context)
        {
            context.Result =
               new ObjectResult(
                   jsonApiTransformer.Transform(
                       context.Exception,
                       500));

            context.HttpContext.Response.StatusCode = 500;
        }
    }
}
