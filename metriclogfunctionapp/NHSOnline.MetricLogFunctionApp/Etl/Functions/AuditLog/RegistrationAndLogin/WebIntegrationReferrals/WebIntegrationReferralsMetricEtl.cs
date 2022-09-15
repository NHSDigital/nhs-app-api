using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals
{
    public class WebIntegrationReferralsMetricEtl : AuditLogEtl<WebIntegrationReferralsMetric>
    {
        public WebIntegrationReferralsMetricEtl(IEventsRepository repo, IAuditLogParser<WebIntegrationReferralsMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {
        }

        protected override string StoredProcedureName =>
            "CALL events.WebIntegrationReferralsInsert({0},{1},{2},{3},{4});";
        protected override object[] ReturnParams(WebIntegrationReferralsMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.IntegrationReferrer,
                metric.ReferrerOrigin,
                metric.SessionId,
                metric.AuditId
            };
        }
    }
}
