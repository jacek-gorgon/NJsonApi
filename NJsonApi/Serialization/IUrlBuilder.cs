namespace NJsonApi.Serialization
{
    public interface IUrlBuilder
    {
        string RoutePrefix { set; get; }
        string GetFullyQualifiedUrl(string urlTemplate);
    }
}