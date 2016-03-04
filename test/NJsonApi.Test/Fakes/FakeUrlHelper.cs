using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Routing;

namespace NJsonApi.Test.Fakes
{
    public class FakeUrlHelper : IUrlHelper
    {
        public string Action(UrlActionContext actionContext) => string.Empty;
        public string Content(string contentPath) => string.Empty;
        public bool IsLocalUrl(string url) => true;
        public string Link(string routeName, object values) => string.Empty;
        public string RouteUrl(UrlRouteContext routeContext) => string.Empty;
    }
}
