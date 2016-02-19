using Newtonsoft.Json;
using NJsonApi.Serialization.Converters;

namespace NJsonApi.Serialization.Representations.Relationships
{
    /// <summary>
    /// This "null-object" pattern is used to distinguish null resource linkage from lack of resource linkage which, according to spec, signifies two different states.
    /// 
    /// E.g. 
    /// relationships: {
    ///   author: {
    ///     links: { 
    ///       self: "articles/1/author"
    ///     }
    ///     data: null
    ///   }
    /// }
    /// 
    /// is different from:
    /// 
    /// relationships: {
    ///   author: {
    ///     links: { 
    ///       self: "articles/1/author"
    ///     }
    ///   }
    /// }
    /// 
    /// The former meaning there is no author attached, while the latter meaning there is no author included, but there may or may not be an author attached.
    /// 
    /// With Newtonsoft.Json, attributes allow to either always serialize a null field as null or always omit it. 
    /// However, choosing behavior dynamically requires this workaround.
    /// </summary>
    [JsonConverter(typeof(SerializationAwareConverter))]
    internal class NullResourceIdentifier : IResourceLinkage, ISerializationAware
    {
        public void Serialize(JsonWriter writer)
        {
            writer.WriteNull();
        }
    }
}
