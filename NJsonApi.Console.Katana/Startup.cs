using NJsonApi.HelloWorld.Common;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NJsonApi.Console.Katana
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureWebApi(app);
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            var webApiRoute = config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}", 
                new { id = RouteParameter.Optional });

            webApiRoute.DataTokens["Namespaces"] = new string[] { "NJsonApi.HelloWorld.Common.Controllers" };

            var nJsonApiConfig = NJsonApiConfiguration.BuildConfiguration();
            nJsonApiConfig.Apply(config);

            app.UseWebApi(config);

        }
    }
}
