using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NJsonApi.Common.Infrastructure;
using NJsonApi.Serialization.Documents;

namespace NJsonApi.Serialization
{
    public class JsonApiActionFilter : IActionFilter
    {
        public bool AllowMultiple { get { return false; } }
        private readonly JsonApiTransformer jsonApiTransformer;
        private readonly Configuration configuration;

        public JsonApiActionFilter(JsonApiTransformer jsonApiTransformer, Configuration configuration)
        {
            this.jsonApiTransformer = jsonApiTransformer;
            this.configuration = configuration;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            InternalActionExecuting(actionContext, cancellationToken);

            if (actionContext.Response != null)
            {
                return actionContext.Response;
            }

            HttpActionExecutedContext executedContext;

            try
            {
                var response = await continuation();
                executedContext = new HttpActionExecutedContext(actionContext, null)
                {
                    Response = response
                };
                InternalActionExecuted(executedContext, cancellationToken);
            }
            catch (Exception exception)
            {
                var context = new Context
                {
                    Configuration = configuration,
                    RoutePrefix = string.Empty
                };

                executedContext = new HttpActionExecutedContext(actionContext, exception);
                if (executedContext.Response == null)
                {
                    var nJsonApiBaseException = exception as NJsonApiBaseException;
                    if (nJsonApiBaseException != null)
                    {
                        executedContext.Response = new HttpResponseMessage(nJsonApiBaseException.GetHttpStatusCode());
                        var transformed = jsonApiTransformer.Transform(nJsonApiBaseException, context);
                        var jsonApiFormatter = new JsonApiFormatter(configuration, jsonApiTransformer.Serializer);
                        executedContext.Response.Content = new ObjectContent(transformed.GetType(), transformed, jsonApiFormatter);
                    }
                    else
                    {
                        executedContext.Response = executedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new NJsonApiBaseException("Internal Server Error"));
                        executedContext.Response.Content = new HttpMessageContent(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                    }
                }
            }

            return executedContext.Response;
        }

        public virtual void InternalActionExecuting(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var contentType = actionContext.Request.Content.Headers.ContentType;
            if (contentType != null && contentType.MediaType != JsonApiFormatter.JSON_API_MIME_TYPE)
            {
                return;
            }

            if (actionContext.ActionArguments.Any(a => a.Value is UpdateDocumentTypeWrapper))
            {
                var argument = actionContext.ActionArguments.First(a => a.Value is UpdateDocumentTypeWrapper);
                var updateDocument = argument.Value as UpdateDocumentTypeWrapper;
                if (updateDocument != null)
                {
                    var resultType = updateDocument.Type.GetGenericArguments()[0];
                    var context = new Context
                    {
                        Configuration = configuration,
                        RoutePrefix = GetRoutePrefix(actionContext)
                    };

                    var result = jsonApiTransformer.TransformBack(updateDocument.UpdateDocument, resultType, context);
                    actionContext.ActionArguments[argument.Key] = result;
                }
            }
        }

        public virtual void InternalActionExecuted(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            try
            {
                var objectContent = actionExecutedContext.Response.Content as ObjectContent;
                if (objectContent != null && objectContent.Formatter.GetType() == typeof(JsonApiFormatter))
                {
                    var value = objectContent.Value;
                    var context = new Context
                    {
                        Configuration = configuration,
                        RoutePrefix = GetRoutePrefix(actionExecutedContext)
                    };
                    var transformed = jsonApiTransformer.Transform(value, context);

                    var jsonApiFormatter = new JsonApiFormatter(configuration, jsonApiTransformer.Serializer);
                    actionExecutedContext.Response.Content = new ObjectContent(transformed.GetType(), transformed, jsonApiFormatter);

                    HandlePostRequests(actionExecutedContext, transformed);
                }
            }            
            catch
            {
                // Different kinds of unsupported requests may end up here. Ideally, these should be programmed against to avoid throwing.
            }
        }

        private string GetRoutePrefix(HttpActionContext context)
        {
            return null;
        }

        private string GetRoutePrefix(HttpActionExecutedContext actionExecutedContext)
        {
            var result = String.Empty;

            if (System.Web.HttpContext.Current == null)
            {
                result += "http://localhost/";
            }

            //var routeData = actionExecutedContext.Request.GetRouteData();
            //if (routeData != null)
            //{
            //    if (routeData.Route != null && routeData.Route.DataTokens != null &&
            //        routeData.Route.DataTokens["actions"] != null)
            //    {
            //        var descriptor = ((HttpActionDescriptor[])routeData.Route.DataTokens["actions"])[0].ControllerDescriptor;
            //        var routePrefixAttribute = descriptor.GetCustomAttributes<RoutePrefixAttribute>().FirstOrDefault();                    
            //        if (routePrefixAttribute != null)
            //        {
            //            var prefix = routePrefixAttribute.Prefix;
            //            foreach (var kvp in routeData.Values)
            //            {
            //                prefix = prefix.Replace("{" + kvp.Key + "}", kvp.Value.ToString());
            //            }
            //            result += prefix;
            //        }
            //    }
            //}

            return result;
        }

        private static void HandlePostRequests(HttpActionExecutedContext actionExecutedContext, CompoundDocument transformed)
        {
            if (actionExecutedContext.Request.Method != HttpMethod.Post)
            {
                return;
            }

            var primaryResourceHref = transformed.GetPrimaryResourceHref();
            if (String.IsNullOrEmpty(primaryResourceHref))
            {
                return;
            }

            actionExecutedContext.Response.Headers.Location = new Uri(primaryResourceHref);
            actionExecutedContext.Response.StatusCode = HttpStatusCode.Created;
        }
    }
}