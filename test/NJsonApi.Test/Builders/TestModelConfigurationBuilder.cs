using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NJsonApi;
using NJsonApi.Test.TestModel;
using NJsonApi.Test.TestControllers;

namespace NJsonApi.Test.Builders
{
    internal static class TestModelConfigurationBuilder
    {
        public static ConfigurationBuilder BuilderForEverything
        {
            get
            {
                var builder = new ConfigurationBuilder();
                builder
                    .Resource<Post, PostsController>()
                    .WithAllProperties()
                    .WithLinkTemplate("/posts/{id}");

                builder
                    .Resource<Author, AuthorsController>()
                    .WithAllProperties()
                    .WithLinkTemplate("/authors/{id}");

                builder
                    .Resource<Comment, CommentsController>()
                    .WithAllProperties()
                    .WithLinkTemplate("/comments/{id}");

                return builder;
            }
        }
    }
}
