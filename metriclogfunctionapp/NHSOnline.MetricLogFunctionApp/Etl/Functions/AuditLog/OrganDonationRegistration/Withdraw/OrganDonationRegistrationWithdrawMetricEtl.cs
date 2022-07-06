using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Withdraw
{
    public class OrganDonationRegistrationWithdrawMetricEtl : AuditLogEtl<OrganDonationRegistrationWithdrawMetric>
    {
        public OrganDonationRegistrationWithdrawMetricEtl(IEventsRepository repo, IAuditLogParser<OrganDonationRegistrationWithdrawMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser,queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.OrganDonationRegistrationWithdrawMetricInsert({0},{1},{2})";

        protected override object[] ReturnParams(OrganDonationRegistrationWithdrawMetric metric)
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
