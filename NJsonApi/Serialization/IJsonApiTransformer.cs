using System;
using Newtonsoft.Json;
using SocialCee.Framework.Common.Infrastructure;

namespace SocialCee.Framework.NJsonApi.Serialization
{
    public interface IJsonApiTransformer
    {
        JsonSerializer Serializer { get; set; }
        CompoundDocument Transform(object objectGraph, Configuration config, string routePrefix);
        IDelta TransformBack(UpdateDocument updateDocument, Configuration config, Type type);
    }
}