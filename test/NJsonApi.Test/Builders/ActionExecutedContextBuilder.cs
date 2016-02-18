using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using NJsonApi.Test.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Test.Builders
{
    internal class ActionExecutedContextBuilder
    {
        private readonly ActionContext actionContext;
        private readonly FakeHttpContext fakeHttpContext;
        private readonly ActionExecutedContext actionExecutedContext;

        public ActionExecutedContextBuilder()
        {
            fakeHttpContext = new FakeHttpContext();

            this.actionContext = new ActionContext()
            {
                HttpContext = fakeHttpContext,
                RouteData = new Microsoft.AspNet.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNet.Mvc.Abstractions.ActionDescriptor()
            };

            actionExecutedContext = new ActionExecutedContext(
                actionContext, new List<IFilterMetadata>(), new { });
        }

        public ActionExecutedContextBuilder WithResult(IActionResult result)
        {
            this.actionExecutedContext.Result = result;
            return this;
        }

        public ActionExecutedContext Build()
        {
            return actionExecutedContext;
        }
    }
}
