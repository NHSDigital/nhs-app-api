using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals
{
    [Table("WebIntegrationReferrals", Schema = "events")]
    public class WebIntegrationReferralsMetric : IEventRepositoryRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string IntegrationReferrer { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }
    }
}