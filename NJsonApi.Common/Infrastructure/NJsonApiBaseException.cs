using System;
using System.Net;
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

        protected NJsonApiBaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
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

        /// <summary>
        /// Return the default mapping of exception to standard HTTP error code. Default implementation returns 500. Sub-classes should override this when needed 
        /// </summary>
        /// <returns></returns>
        public virtual HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
