namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Update
{
    public class OrganDonationRegistrationUpdateEventParser : IAuditLogParser<OrganDonationRegistrationUpdateMetric>
    {
        private const string OperationFieldValue = "OrganDonation_Update_Response";
        private const string DetailsFieldValue = "The organ donation decision has been successfully updated";

        public OrganDonationRegistrationUpdateMetric Parse(AuditRecord source)
        {
            if (!IsOrganDonationRegistrationUpdateMetric(source)) return null;

            return new OrganDonationRegistrationUpdateMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsOrganDonationRegistrationUpdateMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}
