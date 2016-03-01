using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace NJsonApi.Test.Fakes
{
    internal class FakeHttpRequest : HttpRequest
    {
        private readonly FakeHttpContext fakeHttpContext;
        private readonly HeaderDictionary headerDictionary = new HeaderDictionary();
        private readonly Dictionary<string, StringValues> queryStrings = new Dictionary<string, StringValues>();

        public FakeHttpRequest(FakeHttpContext context)
        {
            this.fakeHttpContext = context;
            this.IsHttps = false;
            this.Scheme = "http";
            this.Host = new HostString("localhost");
            this.PathBase = new PathString("");
            this.Path = new PathString("/api/fake");
            this.Query = new ReadableStringCollection(queryStrings);
            this.Body = new MemoryStream();
        }

        public override Stream Body { get; set; }

        public override long? ContentLength
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string ContentType { get; set; }

        public override IReadableStringCollection Cookies
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override IFormCollection Form
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool HasFormContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IHeaderDictionary Headers => headerDictionary;

        public override HostString Host { get; set; }

        public override HttpContext HttpContext
        {
            get
            {
                return fakeHttpContext;
            }
        }

        public override bool IsHttps { get; set; }

        public override string Method
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override PathString Path { get; set; }
        
        public override PathString PathBase { get; set; }

        public override string Protocol
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override IReadableStringCollection Query { get; set; }

        public override QueryString QueryString { get; set; }
        
        public override string Scheme { get; set; }
        

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void AddQuery(string key, string value)
        {
            this.queryStrings.Add(key, value);
        }
    }
}
