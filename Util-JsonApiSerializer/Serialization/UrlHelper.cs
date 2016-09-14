using System;
using System.Linq;
using System.Web;

namespace UtilJsonApiSerializer.Serialization
{
    public class UrlBuilder : IUrlBuilder
    {
        private string routePrefix = string.Empty;
        private string root = string.Empty;

        public string RoutePrefix
        {
            set { routePrefix = value; }
            get
            {
                if (!string.IsNullOrWhiteSpace(routePrefix))
                {
                    return routePrefix;
                }

                return string.Empty;
            }
        }

        public string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(routePrefix))
                {
                    root = routePrefix;
                }
                else
                {
                    if (HttpContext.Current != null)
                    {
                        if (routePrefix == string.Empty)
                        {
                            Uri url = HttpContext.Current.Request.Url;
                            var scheme = url.Scheme;
                            if (HttpContext.Current.Request.Headers["X-Forwarded-Proto"] != null)
                            {
                                scheme = HttpContext.Current.Request.Headers["X-Forwarded-Proto"];
                            }
                            root = scheme + "://" + url.Authority + HttpContext.Current.Request.ApplicationPath;
                        }
                    }
                }
             
                if (!root.EndsWith("/")) root += "/";
                return root;
            }
        }

        public string GetFullyQualifiedUrl(string urlTemplate)
        {
            if (String.IsNullOrEmpty(Url))
            {
                if (urlTemplate.StartsWith("//"))
                    return new Uri(RoutePrefix + urlTemplate.TrimStart('/')).ToString();

                return new Uri(RoutePrefix + urlTemplate).ToString();
            }

            Uri fullyQualiffiedUrl;

            //try to create an absolute URL from the urlTemplate
            if (Uri.TryCreate(urlTemplate, UriKind.Absolute, out fullyQualiffiedUrl))
                return fullyQualiffiedUrl.ToString();

            //try to create an absolute url from the routeprefix + urltemplate
            if (Uri.TryCreate(RoutePrefix + urlTemplate, UriKind.Absolute, out fullyQualiffiedUrl))
                return fullyQualiffiedUrl.ToString();

            if (!Uri.TryCreate(new Uri(Url), new Uri(RoutePrefix + urlTemplate, UriKind.Relative), out fullyQualiffiedUrl))
                throw new ArgumentException(string.Format("Unable to create fully qualified url for urltemplate = '{0}'", urlTemplate));

            return fullyQualiffiedUrl.ToString();
        }
    }
}