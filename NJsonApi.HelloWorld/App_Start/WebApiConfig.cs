using System.Net.Http.Formatting;
using System.Web.Http;
using NJsonApi.HelloWorld.Models;

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
            var configBuilder = new ConfigurationBuilder();

            configBuilder
                .Resource<World>()
                .WithAllProperties();

            configBuilder
                .Resource<Continent>()
                .WithAllProperties();

            var nJsonApiConfig = configBuilder.Build();
            nJsonApiConfig.Apply(config);
        }
    }
}
