namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login
{
    public class LoginEventParser : IAuditLogParser<LoginMetric>
    {
        private const string OperationFieldValue = "Login_Success";
        private const string DetailsFieldValue = "Successful Login with";
        private const string DefaultOdsCode = "Z00001";

        public LoginMetric Parse(AuditRecord source)
        {

            if (!IsLoginMetric(source)) return null;

            return new LoginMetric
            {
                Timestamp = source.Timestamp,
                OdsCode = string.IsNullOrWhiteSpace(source.ODS) ? DefaultOdsCode : source.ODS,
                LoginId = source.NhsLoginSubject,
                ProofLevel = source.ProofLevel,
                Referrer = source.Referrer,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsLoginMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   (source.Details != null && source.Details.Contains(DetailsFieldValue));
        }
    }
}
