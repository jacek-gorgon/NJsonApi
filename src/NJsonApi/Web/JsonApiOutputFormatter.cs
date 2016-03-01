using Microsoft.AspNet.Mvc.Formatters;

namespace NJsonApi.Web
{
    internal class JsonApiOutputFormatter : JsonOutputFormatter
    {
        public JsonApiOutputFormatter(IConfiguration configuration)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(configuration.DefaultJsonApiMediaType);
        }
    }
}
