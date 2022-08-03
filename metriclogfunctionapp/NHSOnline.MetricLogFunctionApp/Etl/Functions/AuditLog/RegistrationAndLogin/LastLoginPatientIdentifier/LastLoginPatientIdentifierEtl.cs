using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;

public class LastLoginPatientIdentifierEtl : AuditLogEtl<LastLoginPatientIdentifier>
{
    public LastLoginPatientIdentifierEtl(
        IEventsRepository repo,
        IAuditLogParser<LastLoginPatientIdentifier> parser,
        IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
        : base(repo, parser, queueOrchestrator)
    {
    }

    protected override string StoredProcedureName =>
        "CALL events.LastLoginPatientIdentifierInsert({0},{1},{2},{3})";

    protected override object[] ReturnParams(LastLoginPatientIdentifier metric)
    {
        return new object[]
        {
            metric.LoginId,
            metric.NhsNumber,
            metric.Timestamp,
            metric.AuditId
        };
    }
}