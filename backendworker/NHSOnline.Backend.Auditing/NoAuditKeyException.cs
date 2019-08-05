using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Auditing
{
    [Serializable]
    public class NoAuditKeyException : Exception
    {
        public NoAuditKeyException()
        {
        }

        public NoAuditKeyException(string message) : base(message)
        {
        }

        public NoAuditKeyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NoAuditKeyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}