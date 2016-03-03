using System;

namespace NJsonApi.Serialization
{
    internal class UrlBuilder : IUrlBuilder
    {
        private string routePrefix = string.Empty;
        private string root = string.Empty;

        public string GetFullyQualifiedUrl(Context context, string urlTemplate)
        {
            var fullUri = new Uri(context.BaseUri, urlTemplate);

            return fullUri.AbsoluteUri;
        }
    }
}