using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations.Resources;

namespace NJsonApi.Serialization
{
    internal class UpdateDocument
    {
        [JsonExtensionData]
        public SingleResource Data { get; set; }
    }
}
