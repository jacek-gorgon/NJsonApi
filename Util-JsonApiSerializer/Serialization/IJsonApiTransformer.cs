using System;
using Newtonsoft.Json;
using UtilJsonApiSerializer.Common.Infrastructure;
using UtilJsonApiSerializer.Serialization.Documents;

namespace UtilJsonApiSerializer.Serialization
{
    public interface IJsonApiTransformer
    {
        JsonSerializer Serializer { get; set; }
        CompoundDocument Transform(object objectGraph, Context context);
        IDelta TransformBack(UpdateDocument updateDocument, Type type, Context context);
    }
}