using System;

namespace NJsonApi.Serialization
{
    public interface IUrlBuilder
    {
        string GetFullyQualifiedUrl(Context context, string urlTemplate);
    }
}