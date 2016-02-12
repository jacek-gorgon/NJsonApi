namespace NJsonApi.Serialization.Representations.Resources
{
    public interface IResourceIdentifier
    {
        string Type { get; set; }
        string Id { get; set; }
    }
}
