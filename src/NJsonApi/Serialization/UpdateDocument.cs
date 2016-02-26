using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations.Resources;

namespace NJsonApi.Serialization
{
    internal class UpdateDocument
    {
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public SingleResource Data { get; set; }
    }
}
