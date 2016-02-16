using NJsonApi.HelloWorld.Models;

namespace NJsonApi.HelloWorld
{
    public static class NJsonApiConfiguration
    {
        public static Configuration BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder
                .Resource<World>()
                .WithAllProperties()
                .WithLinkTemplate("api/worlds/{id}");

            configBuilder
                .Resource<Continent>()
                .WithAllProperties()
                .WithLinkTemplate("api/continents/{id}");

            var nJsonApiConfig = configBuilder.Build();
            return nJsonApiConfig;
        }
    }
}
