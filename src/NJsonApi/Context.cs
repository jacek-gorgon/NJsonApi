using System;

namespace NJsonApi
{
    public class Context
    {
        public Context(Uri requestUri)
        {
            IncludedResources = new string[0];
            RequestUri = requestUri;
        }

        public Context(Uri requestUri, string[] includedResources)
        {
            RequestUri = requestUri;
            IncludedResources = includedResources;
        }

        public Uri RequestUri { get; private set; }
        public string[] IncludedResources { get; set; }

        public Uri BaseUri {
            get
            {
                var authority = (UriComponents.Scheme | UriComponents.UserInfo | UriComponents.Host | UriComponents.Port);
                return new Uri(RequestUri.GetComponents(authority, UriFormat.SafeUnescaped));
            }
        }
    }
}
