using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
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

        public string SessionId { get; set; }

        public string ProofLevel { get; set; }

        public string ODS { get; set; }

        public string Referrer { get; set; }

        public string IntegrationReferrer { get; set; }
    }
}
