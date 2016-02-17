using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NJsonApi;

namespace NJsonApi.Test.TestModel
{
    internal static class TestModelConfigurationBuilder
    {
        public static ConfigurationBuilder BuilderForEverything
        {
            get
            {
                var builder = new ConfigurationBuilder();
                builder
                    .Resource<Post>()
                    .WithAllProperties()
                    .WithLinkTemplate("/posts/{id}");

                builder
                    .Resource<Author>()
                    .WithAllProperties()
                    .WithLinkTemplate("/authors/{id}");

                builder
                    .Resource<Comment>()
                    .WithAllProperties()
                    .WithLinkTemplate("/comments/{id}");

                return builder;
            }
        }
    }
}
