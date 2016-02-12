using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Serialization
{
    public class JsonApiInputFormatter : InputFormatter
    {
        public readonly MediaTypeHeaderValue DefaultJsonApiMimeType = new MediaTypeHeaderValue("application/vnd.api+json");
        private readonly JsonSerializer jsonSerializer;
        private readonly Configuration configuration;
        private readonly JsonApiTransformer jsonApiTransformer;

        public JsonApiInputFormatter(JsonSerializer jsonSerializer, Configuration configuration, JsonApiTransformer jsonApiTransformer)
        {
            this.jsonSerializer = jsonSerializer;
            this.configuration = configuration;
            this.jsonApiTransformer = jsonApiTransformer;
            SupportedMediaTypes.Add(DefaultJsonApiMimeType);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var updateDocument = jsonSerializer.Deserialize(jsonReader, typeof(UpdateDocument)) as UpdateDocument;

                    if (updateDocument != null)
                    {
                        var resultType = context.ModelType.GenericTypeArguments.Single();
                        var jsonApiContext = new Context(configuration, new Uri(context.HttpContext.Request.Host.Value, UriKind.Absolute));

                        var transformed = jsonApiTransformer.TransformBack(updateDocument, resultType, jsonApiContext);

                        return InputFormatterResult.SuccessAsync(transformed);
                    }
                    throw new NotImplementedException("Throw a better error when the update document could not be deserialised, such as a bad request");
                }
            }
        }
    }
}
