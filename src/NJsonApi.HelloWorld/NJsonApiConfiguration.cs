using NJsonApi.HelloWorld.Controllers;
using NJsonApi.HelloWorld.Models;

namespace NJsonApi.HelloWorld
{
    public static class NJsonApiConfiguration
    {
        public static Configuration BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder
                .Resource<Article, ArticlesController>()
                .WithAllProperties()
                .WithLinkTemplate("articles/{id}");

            configBuilder
                .Resource<Person, PeopleController>()
                .WithAllProperties()
                .WithLinkTemplate("people/{id}");

            configBuilder
                .Resource<Comment, CommentsController>()
                .WithAllProperties()
                .WithLinkTemplate("comments/{id}");
            
            var nJsonApiConfig = configBuilder.Build();
            return nJsonApiConfig;
        }
    }
}
