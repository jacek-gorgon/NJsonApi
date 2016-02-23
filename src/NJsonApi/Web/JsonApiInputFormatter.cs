using Microsoft.AspNet.Mvc.Formatters;
using Newtonsoft.Json;
using NJsonApi.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Web
{
    internal class JsonApiInputFormatter : InputFormatter
    {
        private readonly JsonSerializer jsonSerializer;
        private readonly Configuration configuration;
        private readonly JsonApiTransformer jsonApiTransformer;

        public JsonApiInputFormatter(JsonSerializer jsonSerializer, Configuration configuration, JsonApiTransformer jsonApiTransformer)
        {
            this.jsonSerializer = jsonSerializer;
            this.configuration = configuration;
            this.jsonApiTransformer = jsonApiTransformer;

            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(configuration.DefaultJsonApiMediaType);
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
