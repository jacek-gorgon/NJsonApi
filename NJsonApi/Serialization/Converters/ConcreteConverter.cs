using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace NJsonApi.Serialization.Converters
{
    public class ConcreteConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            var target = Activator.CreateInstance(objectType);
            if (objectType.GetInterfaces().Contains(typeof(IDictionary)))
            {
                foreach (JProperty child in obj.Children())
                {
                    objectType.GetMethod("Add").Invoke(target, new object[2] { child.Name, child.Value.ToObject<T>() });
                }
            }
            return target;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

    }
}
