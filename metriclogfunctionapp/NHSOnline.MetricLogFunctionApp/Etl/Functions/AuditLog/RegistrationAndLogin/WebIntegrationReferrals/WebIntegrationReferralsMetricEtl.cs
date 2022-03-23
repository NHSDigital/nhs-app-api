using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals
{
    public class WebIntegrationReferralsMetricEtl : AuditLogEtl<WebIntegrationReferralsMetric>
    {
        public WebIntegrationReferralsMetricEtl(IEventsRepository repo, IAuditLogParser<WebIntegrationReferralsMetric> parser)
            : base(repo, parser)
        {
        }

        protected override string StoredProcedureName =>
            "CALL events.WebIntegrationReferralsInsert({0},{1},{2},{3});";
        protected override object[] ReturnParams(WebIntegrationReferralsMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.IntegrationReferrer,
                metric.SessionId,
                metric.AuditId
            };
        }
    }
}
