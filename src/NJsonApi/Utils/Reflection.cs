using NJsonApi.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NJsonApi.Utils
{
    public static class Reflection
    {
        public static Type GetObjectType(object objectGraph)
        {
            Type objectType = objectGraph.GetType();

            if (objectGraph is IMetaDataWrapper)
            {
                objectType = objectGraph.GetType().GetGenericArguments()[0];
            }

            if (typeof(IEnumerable).IsAssignableFrom(objectType) && objectType.GetTypeInfo().IsGenericType)
            {
                objectType = objectType.GetGenericArguments()[0];
            }

            return objectType;
        }
    }
}
