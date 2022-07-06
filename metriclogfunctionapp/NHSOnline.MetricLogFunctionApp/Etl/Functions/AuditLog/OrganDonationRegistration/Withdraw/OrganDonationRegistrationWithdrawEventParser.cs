namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Withdraw
{
    public class OrganDonationRegistrationWithdrawEventParser : IAuditLogParser<OrganDonationRegistrationWithdrawMetric>
    {
        private const string OperationFieldValue = "OrganDonation_Withdraw_Response";
        private const string DetailsFieldValue = "The organ donation decision has been successfully Withdrawn";

        public OrganDonationRegistrationWithdrawMetric Parse(AuditRecord source)
        {
            if (!IsOrganDonationRegistrationWithdrawMetric(source)) return null;

            return new OrganDonationRegistrationWithdrawMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsOrganDonationRegistrationWithdrawMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}
