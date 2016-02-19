using System.Collections.Generic;
using Newtonsoft.Json;

namespace NJsonApi.Serialization
{
    internal class UpdateDocument
    {
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }
    }
}
