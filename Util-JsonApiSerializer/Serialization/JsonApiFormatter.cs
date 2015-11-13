using System.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using UtilJsonApiSerializer.Common.Infrastructure;
using UtilJsonApiSerializer.Serialization.Documents;
using UtilJsonApiSerializer.Serialization.Converters;

namespace UtilJsonApiSerializer.Serialization
{
    public class JsonApiFormatter : BufferedMediaTypeFormatter
    {
        public const string JSON_API_MIME_TYPE = "application/vnd.api+json";
        private readonly Configuration configuration;
        private readonly JsonSerializer jsonSerializer;
 
        public JsonApiFormatter(Configuration cfg, JsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
            configuration = cfg;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(JSON_API_MIME_TYPE));
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
                var updateDocument = jsonSerializer.Deserialize(jsonReader, typeof (UpdateDocument)) as UpdateDocument;
                ValidateUpdateDocument(updateDocument);

                return new UpdateDocumentTypeWrapper(updateDocument, type);
            }
        }

        public override bool CanReadType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Delta<>);
        }

        public override bool CanWriteType(Type type)
        {
            if (type == typeof (CompoundDocument))
            {
                return true;
            }

            if (type == typeof (HttpError))
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

    
}