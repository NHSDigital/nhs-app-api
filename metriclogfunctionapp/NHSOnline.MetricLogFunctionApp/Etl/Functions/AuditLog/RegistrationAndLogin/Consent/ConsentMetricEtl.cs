using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent
{
    public class ConsentMetricEtl : AuditLogEtl<ConsentMetric>
    {
        public ConsentMetricEtl(IEventsRepository repo, IAuditLogParser<ConsentMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {
        }

        protected override string StoredProcedureName =>
            "CALL events.ConsentMetricInsert({0},{1},{2},{3},{4})";
        protected override object[] ReturnParams(ConsentMetric metric)
        {
            return new object[]
            {
            metric.Timestamp,
            metric.SessionId,
            metric.AuditId,
            metric.LoginId,
            metric.ProofLevel
            };
        }
    }
}

