using System.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#else
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
#endif
using System.Text;
using UtilJsonApiSerializer.Common.Infrastructure;
using UtilJsonApiSerializer.Serialization.Documents;
using UtilJsonApiSerializer.Serialization.Converters;

namespace UtilJsonApiSerializer.Serialization
{

    public class JsonApiFormatterValues
    {
        public static readonly string JSON_API_MIME_TYPE = "application/vnd.api+json";
    }

#if NETCOREAPP

    public class JsonApiInputFormatter : TextInputFormatter
    {
        private readonly Configuration _configuration;
        private readonly JsonSerializer _jsonSerializer;

        public JsonApiInputFormatter(Configuration configuration, JsonSerializer jsonSerializer)
        {
            _configuration = configuration;
            _jsonSerializer = jsonSerializer;

            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(JsonApiFormatterValues.JSON_API_MIME_TYPE));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {

            var textReader = context.ReaderFactory(context.HttpContext.Request.Body, encoding);


            var updateDocument = _jsonSerializer.Deserialize(textReader, typeof(UpdateDocument)) as UpdateDocument;
            ValidateUpdateDocument(updateDocument);

            return await InputFormatterResult.SuccessAsync(updateDocument);
        }

        protected override bool CanReadType(Type type)
        {
            return _configuration.IsMappingRegistered(type);
        }

        private void ValidateUpdateDocument(UpdateDocument updateDocument)
        {
            if (updateDocument == null)
            {
                throw new JsonException("Json body can not be empty or whitespace.");
            }

            if (updateDocument.Data == null)
            {
                throw new JsonException("Json body should contain some content.");
            }
        }
    }



    public class JsonApiOutputFormatter : TextOutputFormatter
    {
        private readonly Configuration _configuration;
        private readonly JsonSerializer _jsonSerializer;

        public JsonApiOutputFormatter(Configuration configuration, JsonSerializer jsonSerializer)
        {
            _configuration = configuration;
            _jsonSerializer = jsonSerializer;

            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(JsonApiFormatterValues.JSON_API_MIME_TYPE));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var httpContext = context.HttpContext;
            var buffer = new StringBuilder();
            _jsonSerializer.Serialize(new StringWriter(buffer), context.Object);
            await httpContext.Response.WriteAsync(buffer.ToString(), selectedEncoding);
        }

        protected override bool CanWriteType(Type? type)
        {
            if (type == null)
            {
                return false;
            }

            if (type == typeof(CompoundDocument))
            {
                return true;
            }

            if (type == typeof(Error))
            {
                return true;
            }

            if ((typeof(Exception).IsAssignableFrom(type)))
            {
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MetaDataWrapper<>))
            {
                type = type.GenericTypeArguments[0];
            }

            return _configuration.IsMappingRegistered(type);
        }
    }
#else
    public class JsonApiFormatter : BufferedMediaTypeFormatter
    {
        private readonly Configuration configuration;
        private readonly JsonSerializer jsonSerializer;

        public JsonApiFormatter(Configuration cfg, JsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
            configuration = cfg;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(JsonApiFormatterValues.JSON_API_MIME_TYPE));
            SupportedEncodings.Add(new UTF8Encoding(false, true));

            //if(jsonSerializer.Converters.All(x => x.GetType() != typeof (CompoundDocumentObjectConverter)))
            //    jsonSerializer.Converters.Add(new CompoundDocumentObjectConverter());
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var textWriter = new StreamWriter(writeStream))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonSerializer.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            using (var reader = new StreamReader(readStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var updateDocument = jsonSerializer.Deserialize(jsonReader, typeof(UpdateDocument)) as UpdateDocument;
                ValidateUpdateDocument(updateDocument);

                return new UpdateDocumentTypeWrapper(updateDocument, type);
            }
        }

        public override bool CanReadType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Delta<>);
        }

        public override bool CanWriteType(Type type)
        {
            if (type == typeof(CompoundDocument))
            {
                return true;
            }

            if (type == typeof(HttpError))
            {
                return true;
            }

            if ((typeof(Exception).IsAssignableFrom(type)))
            {
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MetaDataWrapper<>))
            {
                type = type.GenericTypeArguments[0];
            }

            return configuration.IsMappingRegistered(type);
        }

        private void ValidateUpdateDocument(UpdateDocument updateDocument)
        {
            if (updateDocument == null)
            {
                throw new JsonException("Json body can not be empty or whitespace.");
            }

            if (updateDocument.Data == null)
            {
                throw new JsonException("Json body should contain some content.");
            }
        }
    }
#endif

}