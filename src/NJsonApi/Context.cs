using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string BaseUri {
            get
            {

                var authority = (UriComponents.Scheme | UriComponents.UserInfo | UriComponents.Host | UriComponents.Port);
                var baseUri = new Uri(RequestUri.GetComponents(authority, UriFormat.SafeUnescaped));
                return baseUri.AbsoluteUri;
            }
        }
    }
}
