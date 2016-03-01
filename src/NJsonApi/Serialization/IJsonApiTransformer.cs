using System;
using Newtonsoft.Json;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Documents;

namespace NJsonApi.Serialization
{
    internal interface IJsonApiTransformer
    {
        CompoundDocument Transform(Exception e, int httpStatus);
        CompoundDocument Transform(object objectGraph, Context context);
        IDelta TransformBack(UpdateDocument updateDocument, Type type, Context context);
    }
}