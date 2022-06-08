using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.InitialPrompt
{
    [Table("InitialPromptMetric", Schema = "events")]
    public class InitialPromptMetric : IEventRepositoryRow
    {
        public string LoginId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string OptedIn { get; set; }
        public string AuditId { get; set; }
    }
}