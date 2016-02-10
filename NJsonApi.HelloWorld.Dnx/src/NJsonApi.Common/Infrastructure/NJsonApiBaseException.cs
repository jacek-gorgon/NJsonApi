using System;
using System.Runtime.Serialization;

namespace NJsonApi.Common.Infrastructure
{
    public class NJsonApiBaseException : Exception
    {
        public Guid Id { get; private set; }

        public NJsonApiBaseException()
        {
            AssignId();
        }

        public NJsonApiBaseException(string message)
            : base(message)
        {
            AssignId();
        }

        public NJsonApiBaseException(string message, Exception innerException)
            : base(message, innerException)
        {
            AssignId();
        }

        private void AssignId()
        {
            Id = Guid.NewGuid();
        }

        public virtual int GetHttpStatusCode() => 500;
    }
}
