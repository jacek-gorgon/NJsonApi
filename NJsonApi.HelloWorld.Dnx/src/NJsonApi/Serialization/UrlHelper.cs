using System;

namespace NJsonApi.Serialization
{
    public class UrlBuilder : IUrlBuilder
    {
        private string routePrefix = string.Empty;
        private string root = string.Empty;

        public string GetFullyQualifiedUrl(Context context, string urlTemplate)
        {
            var fullUri = new Uri(new Uri(context.BaseUri, UriKind.Absolute), urlTemplate);

            return fullUri.AbsoluteUri;
        }
    }
}