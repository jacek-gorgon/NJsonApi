using Newtonsoft.Json;

namespace UtilJsonApiSerializer.Serialization.Converters
{
    /// <summary>
    /// Implementations of this interface provide custom serialization logic for their own serialization.
    /// </summary>
    public interface ISerializationAware
    {
        void Serialize(JsonWriter writer);
    }
}
