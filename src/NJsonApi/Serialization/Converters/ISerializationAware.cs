using Newtonsoft.Json;

namespace NJsonApi.Serialization.Converters
{
    /// <summary>
    /// Implementations of this interface provide custom serialization logic for their own serialization.
    /// </summary>
    internal interface ISerializationAware
    {
        void Serialize(JsonWriter writer);
    }
}
