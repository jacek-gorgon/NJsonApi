using System;
using System.Collections.Generic;

namespace UtilJsonApiSerializer.Common.Infrastructure
{
    public interface IMetaDataWrapper
    {
        Dictionary<string, object> MetaData { get; }
        object GetValue();
    }
}