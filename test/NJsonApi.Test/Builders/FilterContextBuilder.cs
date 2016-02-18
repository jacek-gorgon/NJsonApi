using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using NJsonApi.Test.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Test.Builders
{
    internal class FilterContextBuilder
    {
        private readonly ActionContext actionContext;
        private readonly FakeHttpContext fakeHttpContext;
        private readonly ExceptionContext exceptionContext;
        private IActionResult result;
        private Exception exception;

        public FilterContextBuilder()
        {
            fakeHttpContext = new FakeHttpContext();

            this.actionContext = new ActionContext()
            {
                HttpContext = fakeHttpContext,
                RouteData = new Microsoft.AspNet.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNet.Mvc.Abstractions.ActionDescriptor()
            };
        }

        public FilterContextBuilder WithResult(IActionResult result)
        {
            this.result = result;
            return this;
        }

        public FilterContextBuilder WithException(string message)
        {
            this.exception = new Exception(message);
            return this;
        }

        public ActionExecutedContext BuildActionExecuted()
        {
            var actionExecutedContext = new ActionExecutedContext(
                actionContext, new List<IFilterMetadata>(), new { });
            actionExecutedContext.Result = result;
            actionExecutedContext.Exception = exception;

            return actionExecutedContext;
        }

        public ExceptionContext BuildException()
        {
            var exceptionContext = new ExceptionContext(
                actionContext, new List<IFilterMetadata>());
            exceptionContext.Result = result;
            exceptionContext.Exception = exception;

            return exceptionContext;
        }
    }
}
