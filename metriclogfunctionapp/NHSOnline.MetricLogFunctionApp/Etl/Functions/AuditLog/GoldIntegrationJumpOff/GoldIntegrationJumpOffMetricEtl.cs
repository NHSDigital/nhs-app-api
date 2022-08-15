using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.GoldIntegrationJumpOff;

public class GoldIntegrationJumpOffMetricEtl : AuditLogEtl<GoldIntegrationJumpOffMetric>
{
    public GoldIntegrationJumpOffMetricEtl(IEventsRepository repo, IAuditLogParser<GoldIntegrationJumpOffMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
        : base(repo, parser, queueOrchestrator)
    {
    }

    protected override string StoredProcedureName =>
        "CALL events.GoldIntegrationJumpOffMetricInsert({0},{1},{2},{3},{4},{5})";

    protected override object[] ReturnParams(GoldIntegrationJumpOffMetric metric)
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
