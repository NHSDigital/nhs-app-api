using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Update
{
    public class OrganDonationRegistrationUpdateMetricEtl : AuditLogEtl<OrganDonationRegistrationUpdateMetric>
    {
        public OrganDonationRegistrationUpdateMetricEtl(IEventsRepository repo, IAuditLogParser<OrganDonationRegistrationUpdateMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser,queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.OrganDonationRegistrationUpdateMetricInsert({0},{1},{2})";

        protected override object[] ReturnParams(OrganDonationRegistrationUpdateMetric metric)
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
