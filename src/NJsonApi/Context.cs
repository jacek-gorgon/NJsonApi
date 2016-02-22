using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi
{
    public class Context
    {
        public Context(Configuration configuration, Uri requestUri)
        {
            Configuration = configuration;
            RequestUri = requestUri;
        }

        public Context(Configuration configuration, Uri requestUri, string[] includedResources)
        {
            Configuration = configuration;
            RequestUri = requestUri;
            IncludedResources = includedResources;
        }

        public Configuration Configuration { get; private set; }
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
