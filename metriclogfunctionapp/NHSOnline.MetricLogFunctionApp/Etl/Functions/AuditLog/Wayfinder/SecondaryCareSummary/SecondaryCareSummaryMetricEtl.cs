using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary
{
    public class SecondaryCareSummaryMetricEtl : AuditLogEtl<SecondaryCareSummaryMetric>
    {
        public SecondaryCareSummaryMetricEtl(IEventsRepository repo, IAuditLogParser<SecondaryCareSummaryMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser,queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.SecondaryCareSummaryMetricInsert({0},{1},{2},{3},{4})";

        protected override object[] ReturnParams(SecondaryCareSummaryMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.SessionId,
                metric.TotalReferrals,
                metric.TotalUpcomingAppointments,
                metric.AuditId
            };
        }
    }
}