using System;

namespace NJsonApi.Serialization
{
    internal interface IUrlBuilder
    {
        string GetFullyQualifiedUrl(Context context, string urlTemplate);
    }
}