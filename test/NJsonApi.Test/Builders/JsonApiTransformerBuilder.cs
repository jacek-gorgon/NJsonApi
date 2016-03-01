using Microsoft.AspNet.Mvc.ApiExplorer;
using Newtonsoft.Json;
using NJsonApi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Test.Builders
{
    internal class JsonApiTransformerBuilder
    {
        private readonly IApiDescriptionGroupCollectionProvider apiProvider;
        private IConfiguration config;

        public JsonApiTransformerBuilder()
        {
            this.apiProvider = null;
        }

        public JsonApiTransformerBuilder With(IConfiguration config)
        {
            this.config = config;
            return this;
        }

        public JsonApiTransformer Build()
        {
            var serializer = JsonSerializerBuilder.Build();
            return new JsonApiTransformer(serializer, apiProvider, config);
        }
    }
}
