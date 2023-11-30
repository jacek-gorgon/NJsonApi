

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
#if NETCOREAPP

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
#else
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
#endif
using UtilJsonApiSerializer.Common.Infrastructure;
using UtilJsonApiSerializer.Serialization.Documents;

namespace UtilJsonApiSerializer.Serialization
{

#if NETCOREAPP
    public class JsonApiActionFilter : IAsyncActionFilter
#else
    public class JsonApiActionFilter : IActionFilter
#endif

    {
        public bool AllowMultiple { get { return false; } }
        private readonly JsonApiTransformer jsonApiTransformer;
        private readonly Configuration configuration;

        public JsonApiActionFilter(JsonApiTransformer jsonApiTransformer, Configuration configuration)
        {
            this.jsonApiTransformer = jsonApiTransformer;
            this.configuration = configuration;
        }




#if NETCOREAPP


        public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            InternalActionExecuting(actionContext);
            if (actionContext.HttpContext.Response != null) return;

            ActionExecutedContext executedContext;
            try
            {
                var response = await next();
                executedContext = new ActionExecutedContext(actionContext, null, actionContext.Controller)
                {
                    Result = response.Result
                };
                InternalActionExecuted(executedContext);
            }
            catch (Exception exception)
            {
                var context = new Context
                {
                    Configuration = configuration,
                    RoutePrefix = string.Empty
                };

                executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), actionContext.Controller);
                //executedContext = new HttpActionExecutedContext(actionContext, exception);
                if (executedContext.HttpContext.Response == null)
                {
                    var UtilJsonApiSerializerBaseException = exception as UtilJsonApiSerializerBaseException;
                    if (UtilJsonApiSerializerBaseException != null)
                    {

                        var transformed = jsonApiTransformer.Transform(UtilJsonApiSerializerBaseException, context);
                        var jsonApiFormatter = new JsonApiOutputFormatter(configuration, jsonApiTransformer.Serializer); var objectResult = new ObjectResult(transformed)
                        {
                            Formatters = new FormatterCollection<IOutputFormatter>(new List<IOutputFormatter> { jsonApiFormatter }),
                            StatusCode = (int)HttpStatusCode.OK
                        };

                        executedContext.Result = objectResult;
                    }
                    else
                    {
                        executedContext.Result = new ObjectResult(exception)
                        {
                            StatusCode = (int)HttpStatusCode.InternalServerError,
                            Formatters = new FormatterCollection<IOutputFormatter>(new List<IOutputFormatter> { new JsonApiOutputFormatter(configuration, jsonApiTransformer.Serializer) })
                        };
                    }
                }
            }
        }

        public virtual void InternalActionExecuting(ActionExecutingContext actionContext)
        {
            var contentType = actionContext.HttpContext.Request.ContentType;


            if (!string.IsNullOrEmpty(contentType) && contentType != JsonApiFormatterValues.JSON_API_MIME_TYPE) return;


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
                        RoutePrefix = string.Empty
                    };

                    var result = jsonApiTransformer.TransformBack(updateDocument.UpdateDocument, resultType, context);
                    actionContext.ActionArguments[argument.Key] = result;
                }
            }
        }

        public virtual void InternalActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                var objectContent = actionExecutedContext.Result as ObjectResult;

                if (objectContent != null && objectContent.Formatters.Any(f => f.GetType() == typeof(JsonApiOutputFormatter)))
                {
                    var value = objectContent.Value;
                    var context = new Context
                    {
                        Configuration = configuration,
                        RoutePrefix = GetRoutePrefix(actionExecutedContext)
                    };
                    var transformed = jsonApiTransformer.Transform(value, context);

                    var jsonApiFormatter = new JsonApiOutputFormatter(configuration, jsonApiTransformer.Serializer);

                    var objectResult = new ObjectResult(transformed)
                    {
                        Formatters = new FormatterCollection<IOutputFormatter>(new List<IOutputFormatter> { jsonApiFormatter }),
                        StatusCode = (int)HttpStatusCode.OK
                    };

                    actionExecutedContext.Result = objectResult;

                    HandlePostRequests(actionExecutedContext, transformed);
                }
            }
            catch
            {
                // Different kinds of unsupported requests may end up here. Ideally, these should be programmed against to avoid throwing.
            }
        }


        private static void HandlePostRequests(ActionExecutedContext actionExecutedContext, CompoundDocument transformed)
        {

            // If the request is not a POST request, we don't need to do anything.

            if (actionExecutedContext.HttpContext.Request.Method.ToLower() != "post") return;




            var primaryResourceHref = transformed.GetPrimaryResourceHref();
            if (string.IsNullOrEmpty(primaryResourceHref)) return;

            actionExecutedContext.HttpContext.Response.Headers.Location = new Uri(primaryResourceHref).AbsolutePath;
            actionExecutedContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        }

        private string GetRoutePrefix(ActionExecutedContext actionExecutedContext)
        {
            var result = String.Empty;

            if (actionExecutedContext.HttpContext == null)
            {
                result += "http://localhost/";
            }


            return result;
        }
#else
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
                    var UtilJsonApiSerializerBaseException = exception as UtilJsonApiSerializerBaseException;
                    if (UtilJsonApiSerializerBaseException != null)
                    {
                        executedContext.Response = new HttpResponseMessage(UtilJsonApiSerializerBaseException.GetHttpStatusCode());
                        var transformed = jsonApiTransformer.Transform(UtilJsonApiSerializerBaseException, context);
                        var jsonApiFormatter = new JsonApiFormatter(configuration, jsonApiTransformer.Serializer);
                        executedContext.Response.Content = new ObjectContent(transformed.GetType(), transformed, jsonApiFormatter);
                    }
                    else
                    {
                        executedContext.Response = executedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new UtilJsonApiSerializerBaseException("Internal Server Error"));
                        executedContext.Response.Content = new HttpMessageContent(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                    }
                }
            }

            return executedContext.Response;
        }

        public virtual void InternalActionExecuting(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var contentType = actionContext.Request.Content.Headers.ContentType;
            if (contentType != null && contentType.MediaType != JsonApiFormatterValues.JSON_API_MIME_TYPE)
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
#endif

    }
}