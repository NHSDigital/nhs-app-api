using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login
{
    [Table("LoginMetric", Schema = "events")]
    public class LoginMetric : IEventRepositoryRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string OdsCode { get; set; }
        public string LoginId { get; set; }
        public string ProofLevel { get; set; }
        public string LoginEventId { get; set; }
        public string Referrer { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }
    }
}