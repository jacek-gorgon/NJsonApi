using System.Collections.Generic;
using Newtonsoft.Json;

namespace NJsonApi.Serialization
{
    public class UpdateDocument
    {
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }
    }
}
