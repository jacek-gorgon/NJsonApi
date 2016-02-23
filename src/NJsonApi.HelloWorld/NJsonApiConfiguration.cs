using NJsonApi.HelloWorld.Models;

namespace NJsonApi.HelloWorld
{
    public static class NJsonApiConfiguration
    {
        public static Configuration BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder
                .Resource<Article>()
                .WithAllProperties()
                .WithLinkTemplate("articles/{id}");

            configBuilder
                .Resource<Person>()
                .WithAllProperties()
                .WithLinkTemplate("people/{id}");

            configBuilder
                .Resource<Comment>()
                .WithAllProperties()
                .WithLinkTemplate("comments/{id}");
            
            var nJsonApiConfig = configBuilder.Build();
            return nJsonApiConfig;
        }
    }
}
