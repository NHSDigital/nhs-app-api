namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent
{
    public class ConsentEventParser : IAuditLogParser<ConsentMetric>
    {
        private const string OperationFieldValue = "TermsAndConditions_RecordConsent_Response";
        private const string DetailsFieldValue = "Initial Consent Successfully recorded";

        public ConsentMetric Parse(AuditRecord source)
        {
            if (!IsConsentMetric(source)) return null;

            return new ConsentMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId,
                LoginId = source.NhsLoginSubject,
                ProofLevel = source.ProofLevel
            };
        }

        private bool IsConsentMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}

