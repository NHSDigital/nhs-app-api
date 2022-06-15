using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Cancel
{
    public class AppointmentCancelMetricEtl : AuditLogEtl<AppointmentCancelMetric>
    {
        public AppointmentCancelMetricEtl(IEventsRepository repo, IAuditLogParser<AppointmentCancelMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.AppointmentCancelMetricInsert({0},{1},{2})";

        protected override object[] ReturnParams(AppointmentCancelMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.SessionId,
                metric.AuditId
            };
        }
    }
}