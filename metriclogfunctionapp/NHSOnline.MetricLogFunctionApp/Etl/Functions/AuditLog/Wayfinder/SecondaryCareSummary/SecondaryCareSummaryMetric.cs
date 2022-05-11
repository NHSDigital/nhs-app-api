using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary
{
    [Table("SecondaryCareSummaryMetric", Schema = "events")]
    public class SecondaryCareSummaryMetric : IEventRepositoryRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public int TotalReferrals { get; set; }
        public int TotalUpcomingAppointments { get; set; }
        public string AuditId { get; set; }
    }
}