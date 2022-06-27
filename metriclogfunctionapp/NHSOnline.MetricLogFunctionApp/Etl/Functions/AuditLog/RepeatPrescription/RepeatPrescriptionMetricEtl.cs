using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RepeatPrescription
{
    public class RepeatPrescriptionMetricEtl : AuditLogEtl<RepeatPrescriptionMetric>
    {
        public RepeatPrescriptionMetricEtl(IEventsRepository repo, IAuditLogParser<RepeatPrescriptionMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {

        }
        
        protected override string StoredProcedureName =>
            "CALL events.PrescriptionOrdersMetricInsert({0},{1},{2})";

        protected override object[] ReturnParams(RepeatPrescriptionMetric metric)
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
