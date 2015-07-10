namespace SocialCee.Framework.NJsonApi
{
    public interface IResourceConfigurationBuilder
    {
        IResourceMapping ConstructedMetadata { get; set; }
    }
}