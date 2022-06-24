namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Create
{
    public class OrganDonationRegistrationCreateEventParser : IAuditLogParser<OrganDonationRegistrationCreateMetric>
    {
        private const string OperationFieldValue = "OrganDonation_Registration_Response";
        private const string DetailsFieldValue = "The organ donation decision has been successfully registered";

        public OrganDonationRegistrationCreateMetric Parse(AuditRecord source)
        {
            if (!IsOrganDonationRegistrationCreateMetric(source)) return null;

            return new OrganDonationRegistrationCreateMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsOrganDonationRegistrationCreateMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}
