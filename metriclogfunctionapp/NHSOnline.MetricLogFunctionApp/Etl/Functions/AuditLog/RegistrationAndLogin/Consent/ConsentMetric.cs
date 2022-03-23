using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent
{
    [Table("ConsentMetric", Schema = "events")]
    public class ConsentMetric : IEventRepositoryRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string OdsCode { get; set; }
        public string LoginId { get; set; }
        public string ProofLevel { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }
    }
}