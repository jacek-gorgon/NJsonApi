using System.Web.Http;
using NJsonApi.HelloWorld.Common;

namespace NJsonApi.HelloWorld
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Clear();

            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // The following code bootstraps NJsonApi
            var nJsonApiConfig = NJsonApiConfiguration.BuildConfiguration();
            nJsonApiConfig.Apply(config);
        }
    }
}
