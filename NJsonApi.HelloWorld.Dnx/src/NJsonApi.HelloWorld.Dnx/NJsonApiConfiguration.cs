using NJsonApi.HelloWorld.Dnx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.HelloWorld.Dnx
{
    public static class NJsonApiConfiguration
    {
        public static Configuration BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder
                .Resource<World>()
                .WithAllProperties()
                .WithLinkTemplate("/worlds/{id}");

            configBuilder
                .Resource<Continent>()
                .WithAllProperties()
                .WithLinkTemplate("/continents/{id}");

            var nJsonApiConfig = configBuilder.Build();
            return nJsonApiConfig;
        }
    }
}
