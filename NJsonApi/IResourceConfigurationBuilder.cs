namespace NJsonApi
{
    public interface IResourceConfigurationBuilder
    {
        IResourceMapping ConstructedMetadata { get; set; }
    }
}