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

        public string AuditId { get; set; }

        public string NhsLoginSubject { get; set; }

        public string NhsNumber { get; set; }

        public bool IsActingOnBehalfOfAnother { get; set; }

        public string Supplier { get; set; }

        public string Operation { get; set; }

        public string Details { get; set; }

        public string ApiVersion { get; set; }

        public string WebVersion { get; set; }

        public string NativeVersion { get; set; }

        public string Environment { get; set; }

        public string IntegrationReferrer { get; set; }

        public string ReferrerOrigin { get; set; }

        public string SessionId { get; set; }

        public string ProofLevel { get; set; }

        public AuditRecord(
            DateTime timestamp,
            string nhsLoginSubject,
            string nhsNumber,
            bool isActingOnBehalfOfAnother,
            string supplier,
            string operation,
            string details,
            VersionTag version,
            string auditId,
            string environment,
            string integrationReferrer,
            string referrerOrigin,
            string sessionId,
            string proofLevel
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
            IntegrationReferrer = integrationReferrer;
            ReferrerOrigin = referrerOrigin;
            Environment = environment;
            SessionId = sessionId;
            ProofLevel = proofLevel;

            if (version != null)
            {
                ApiVersion = version.Api;
                WebVersion = version.Web;
                NativeVersion = version.Native;
            }
        }
    }
}
