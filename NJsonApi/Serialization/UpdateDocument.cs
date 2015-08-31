using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations.Resources;

namespace NJsonApi.Serialization
{
    public class UpdateDocument
    {
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public SingleResource Data { get; set; }
    }
}
