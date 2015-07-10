using System;

namespace SocialCee.Framework.NJsonApi.Exceptions
{
    public class MisssingMappingException : Exception
    {
        public MisssingMappingException(Type type)
        {
            MissingMappingType = type;
        }

        public Type MissingMappingType { get; set; }
    }
}