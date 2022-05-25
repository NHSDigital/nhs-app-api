using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.Toggle
{
    [Table("NotificationToggleMetric", Schema = "events")]
    public class NotificationToggleMetric : IEventRepositoryRow
    {
        public string LoginId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string NotificationToggle { get; set; }
        public string AuditId { get; set; }
    }
}