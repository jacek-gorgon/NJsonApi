using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilJsonApiSerializer
{
    public interface IJsonApiSerializer
    {
        //serializer
        object SerializeObject(ConfigurationBuilder serializerConfig, object obj);
        //deserializer
        T DeserializeObject<T>(string json);

        Dictionary<string, object> GetDocumentProperties(string json);

    }
}
