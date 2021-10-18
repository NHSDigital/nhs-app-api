using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Auditing
{
    [Serializable]
    public class EventHubAuditException : Exception
    {
        public EventHubAuditException()
        {
        }

        public EventHubAuditException(string message) : base(message)
        {
        }

        public EventHubAuditException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EventHubAuditException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}