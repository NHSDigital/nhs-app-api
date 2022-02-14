using System;
using System.Text.Json.Serialization;

namespace NHSOnline.AuditLogFunctionApp.Model
{
    [Serializable]
    public class AuditRecord
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string AuditId { get; private set; }

        public string NhsLoginSubject { get; private set; }

        public string NhsNumber { get; private set; }

        public bool IsActingOnBehalfOfAnother { get; private set; }

        public string Supplier { get; private set; }

        public string Operation { get; private set; }

        public string Details { get; private set; }

        public string ApiVersion { get; private set; }

        public string WebVersion { get; private set; }

        public string NativeVersion { get; private set; }

        public AuditRecord(
            DateTime timestamp,
            string nhsLoginSubject,
            string nhsNumber,
            bool isActingOnBehalfOfAnother,
            string supplier,
            string operation,
            string details,
            VersionTag version,
            string auditId
        )
        {
            Timestamp = timestamp;
            AuditId = auditId;
            NhsLoginSubject = nhsLoginSubject;
            NhsNumber = nhsNumber;
            IsActingOnBehalfOfAnother = isActingOnBehalfOfAnother;
            Supplier = supplier;
            Operation = operation;
            Details = details;

            if (version == null)
            {
                return;
            }

            ApiVersion = version.Api;
            WebVersion = version.Web;
            NativeVersion = version.Native;
        }
    }
}
