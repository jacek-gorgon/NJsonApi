using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using NJsonApi.Serialization.BadActionResultTransformers;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNet.Http;

namespace NJsonApi.Serialization
{
    internal class JsonApiActionFilter : IActionFilter
    {
        public bool AllowMultiple { get { return false; } }
        private readonly JsonApiTransformer jsonApiTransformer;
        private readonly Configuration configuration;

        public JsonApiActionFilter(JsonApiTransformer jsonApiTransformer, Configuration configuration)
        {
            this.jsonApiTransformer = jsonApiTransformer;
            this.configuration = configuration;
        }
    
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.ContentType != configuration.DefaultJsonApiMediaType.MediaType)
            {
                context.Result = new UnsupportedMediaTypeResult();
            }

            if (!ValidateAcceptHeader(context.HttpContext.Request.Headers))
            { 
                context.Result = new HttpStatusCodeResult(406);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result == null)
            {
                return;
            }

            if (BadActionResultTransformer.IsBadAction(context.Result))
            {
                var transformed = BadActionResultTransformer.Transform(context.Result);

                context.Result = new ObjectResult(transformed)
                {
                    StatusCode = transformed.Errors.First().Status
                };
                return;
            }

            var jsonApiContext = new Context(
                configuration, 
                new Uri(context.HttpContext.Request.GetDisplayUrl()),
                FindRelationshipPathsToInclude(context.HttpContext.Request));
            var responseResult = (ObjectResult)context.Result;
            responseResult.Value = jsonApiTransformer.Transform(responseResult.Value, jsonApiContext);
        }

        private string[] FindRelationshipPathsToInclude(HttpRequest request)
        {
            var result = request.Query["include"].FirstOrDefault();

            return string.IsNullOrEmpty(result) ? new string[0] : result.Split(',');
        }

        private bool ValidateAcceptHeader(IHeaderDictionary headers)
        {
            var acceptsHeaders = headers["Accept"].FirstOrDefault();

            if (string.IsNullOrEmpty(acceptsHeaders))
            {
                return true;
            }

            return acceptsHeaders
                .Split(',')
                .Select(x => x.Trim())
                .Any(x => 
                    x == "*/*" || 
                    x == configuration.DefaultJsonApiMediaType.MediaType);
        }
        
    }
}