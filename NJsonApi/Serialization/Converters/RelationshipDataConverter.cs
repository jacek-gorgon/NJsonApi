using System;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations;
using Newtonsoft.Json.Linq;
using NJsonApi.Serialization.Representations.Relationships;

namespace NJsonApi.Serialization.Converters
{
    public class RelationshipDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IResourceLinkage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken obj = JToken.Load(reader);
            switch (obj.Type)
            {   
                case JTokenType.Object:
                    return obj.ToObject<SingleResourceIdentifier>();
                case JTokenType.Array:
                    return obj.ToObject<MultipleResourceIdentifiers>();
                default:
                    throw new InvalidOperationException("When updating a resource, each relationship needs to contain data the is either an array or an object.");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializer.Serialize(writer, value);
        }
    }
}
