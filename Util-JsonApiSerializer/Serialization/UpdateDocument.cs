using System.Collections.Generic;
using Newtonsoft.Json;

namespace UtilJsonApiSerializer.Serialization
{
    public class UpdateDocument
    {
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }
    }
}
