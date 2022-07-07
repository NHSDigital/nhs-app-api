using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.BiometricsToggle;

public class BiometricsToggleMetricEtl : AuditLogEtl<BiometricsToggleMetric>
{
    public BiometricsToggleMetricEtl(IEventsRepository repo, IAuditLogParser<BiometricsToggleMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
        : base(repo, parser, queueOrchestrator)
    {
    }

    protected override string StoredProcedureName =>
        "CALL events.BiometricsToggleMetricInsert({0},{1},{2},{3})";
    protected override object[] ReturnParams(BiometricsToggleMetric metric)
    {
        return new object[]
        {
            metric.SessionId,
            metric.Timestamp,
            metric.BiometricsToggle,
            metric.AuditId
        };
    }
}
