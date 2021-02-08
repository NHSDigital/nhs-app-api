using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Auditing
{
    [Serializable]
    public class PreOperationAuditException : Exception
    {
        public PreOperationAuditException()
        {
        }

        public PreOperationAuditException(string message) : base(message)
        {
        }

        public PreOperationAuditException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PreOperationAuditException(
            SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
