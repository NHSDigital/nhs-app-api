using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.Toggle
{
    public class NotificationToggleMetricEtl : AuditLogEtl<NotificationToggleMetric>
    {
        public NotificationToggleMetricEtl(IEventsRepository repo, IAuditLogParser<NotificationToggleMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.NotificationToggleMetricInsert({0},{1},{2},{3})";

        protected override object[] ReturnParams(NotificationToggleMetric metric)
        {
            return new object[]
            {
                metric.LoginId,
                metric.Timestamp,
                metric.NotificationToggle,
                metric.AuditId
            };
        }
    }
}