using Microsoft.AspNet.Mvc.Formatters;

namespace NJsonApi.Web
{
    internal class JsonApiOutputFormatter : JsonOutputFormatter
    {
        public JsonApiOutputFormatter(Configuration configuration)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(configuration.DefaultJsonApiMediaType);
        }
    }
}
