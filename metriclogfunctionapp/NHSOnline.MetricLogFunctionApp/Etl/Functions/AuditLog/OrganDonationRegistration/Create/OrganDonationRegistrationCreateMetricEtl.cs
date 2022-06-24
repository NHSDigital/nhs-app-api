using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Create
{
    public class OrganDonationRegistrationCreateMetricEtl : AuditLogEtl<OrganDonationRegistrationCreateMetric>
    {
        public OrganDonationRegistrationCreateMetricEtl(IEventsRepository repo, IAuditLogParser<OrganDonationRegistrationCreateMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser,queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.OrganDonationRegistrationCreateMetricInsert({0},{1},{2})";

        protected override object[] ReturnParams(OrganDonationRegistrationCreateMetric metric)
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
