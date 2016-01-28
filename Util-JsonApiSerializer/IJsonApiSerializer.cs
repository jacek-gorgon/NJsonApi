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
    }
}
