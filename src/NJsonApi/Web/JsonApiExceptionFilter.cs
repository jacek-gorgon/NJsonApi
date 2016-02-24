using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using NJsonApi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Web
{
    internal class JsonApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IJsonApiTransformer jsonApiTransformer;

        public JsonApiExceptionFilter(IJsonApiTransformer jsonApiTransformer)
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
