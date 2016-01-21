using System;

namespace UtilJsonApiSerializer.Exceptions
{
    public class MissingMappingException : Exception
    {
        public MissingMappingException(Type type)
        {
            MissingMappingType = type;
        }

        public Type MissingMappingType { get; set; }
    }
}