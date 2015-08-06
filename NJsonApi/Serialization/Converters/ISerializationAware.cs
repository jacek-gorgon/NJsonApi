namespace NJsonApi.Serialization.Converters
{
    /// <summary>
    /// Implementations of this interface provide custom serialization logic for their own serialization.
    /// </summary>
    public interface ISerializationAware
    {
        string Serialize();
    }
}
