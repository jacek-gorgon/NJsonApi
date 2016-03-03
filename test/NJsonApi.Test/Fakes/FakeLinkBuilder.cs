using NJsonApi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Test.Fakes
{
    internal class FakeLinkBuilder : ILinkBuilder
    {
        public ILink FindLink(Context context, string id, IResourceMapping resourceMapping)
        {
            return new SimpleLink(new Uri("http://example.com"));
        }
    }
}
