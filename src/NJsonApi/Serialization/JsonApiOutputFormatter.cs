using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Serialization
{
    public class JsonApiOutputFormatter : JsonOutputFormatter
    {
        public JsonApiOutputFormatter()
            :base()
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/vnd.api+json"));
        }
    }
}
