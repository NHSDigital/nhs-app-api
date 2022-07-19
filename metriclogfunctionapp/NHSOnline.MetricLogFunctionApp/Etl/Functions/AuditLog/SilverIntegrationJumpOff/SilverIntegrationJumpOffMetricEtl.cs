using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.SilverIntegrationJumpOff;

public class SilverIntegrationJumpOffMetricEtl : AuditLogEtl<SilverIntegrationJumpOffMetric>
{
        public SilverIntegrationJumpOffMetricEtl(IEventsRepository repo, IAuditLogParser<SilverIntegrationJumpOffMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {
        }

        protected override string StoredProcedureName =>
            "CALL events.SilverIntegrationJumpOffMetricInsert({0},{1},{2},{3},{4},{5})";

        protected override object[] ReturnParams(SilverIntegrationJumpOffMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.SessionId,
                metric.ProviderId ,
                metric.ProviderName,
                metric.JumpOffId,
                metric.AuditId
            };
        }
}
