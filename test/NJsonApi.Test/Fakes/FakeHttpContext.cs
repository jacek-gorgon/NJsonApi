using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features;
using System.Security.Claims;
using System.Threading;

namespace NJsonApi.Test.Fakes
{
    internal class FakeHttpContext : HttpContext
    {
        private FakeHttpRequest fakeRequest;
        private FakeHttpResponse fakeResponse;

        public FakeHttpContext()
        {
            this.fakeRequest = new FakeHttpRequest(this);
            this.fakeResponse = new FakeHttpResponse();
        }

        public void SetResponse(FakeHttpResponse response)
        {
            this.fakeResponse = response;
        }

        public override IServiceProvider ApplicationServices
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

        public override AuthenticationManager Authentication
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ConnectionInfo Connection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IFeatureCollection Features
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IDictionary<object, object> Items
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

        public override HttpRequest Request
        {
            get
            {
                return fakeRequest;
            }
        }

        public override CancellationToken RequestAborted
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

        public override IServiceProvider RequestServices
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

        public override HttpResponse Response { get { return fakeResponse; } }

        public override ISession Session
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

        public override string TraceIdentifier
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

        public override ClaimsPrincipal User
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

        public override WebSocketManager WebSockets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
