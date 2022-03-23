namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals
{
    public class WebIntegrationReferralsEventParser : IAuditLogParser<WebIntegrationReferralsMetric>
    {
        private const string OperationFieldValue = "Login_Success";
        private const string DetailsFieldValue = "Successful Login with";

        public WebIntegrationReferralsMetric Parse(AuditRecord source)
        {

            if (!IsValidWebIntegrationReferralMetric(source)) return null;

            return new WebIntegrationReferralsMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId,
                IntegrationReferrer = source.IntegrationReferrer
            };
        }

        private bool IsValidWebIntegrationReferralMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue) &&
                   !string.IsNullOrWhiteSpace(source.IntegrationReferrer);
        }
    }
}
