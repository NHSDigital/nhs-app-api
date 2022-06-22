using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Get;

public class OrganDonationRegistrationGetMetricEtl : AuditLogEtl<OrganDonationRegistrationGetMetric>
{
    public OrganDonationRegistrationGetMetricEtl(IEventsRepository repo, IAuditLogParser<OrganDonationRegistrationGetMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator) : base(repo, parser, queueOrchestrator)
    {
    }

    protected override string StoredProcedureName => "CALL events.OrganDonationRegistrationGetMetricInsert({0},{1},{2})";
    protected override object[] ReturnParams(OrganDonationRegistrationGetMetric metric)
    {
        return new object[]
        {
            metric.Timestamp,
            metric.SessionId,
            metric.AuditId,
        };
    }
}
