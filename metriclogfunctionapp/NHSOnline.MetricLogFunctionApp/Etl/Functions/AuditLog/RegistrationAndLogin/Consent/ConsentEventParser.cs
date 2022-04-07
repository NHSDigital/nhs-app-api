namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent
{
    public class ConsentEventParser : IAuditLogParser<ConsentMetric>
    {
        private const string OperationFieldValue = "TermsAndConditions_RecordConsent_Response";
        private const string DetailsFieldValue = "Initial Consent Successfully recorded";

        public ConsentMetric Parse(AuditRecord source)
        {
            if (!IsConsentMetric(source)) return null;

            // We aren't extracting the ODS Code from the Audit Record
            // But we do have it in the metric, and try search on it in int tests.

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

