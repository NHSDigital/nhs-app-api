namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Get;

public class OrganDonationRegistrationGetEventParser : IAuditLogParser<OrganDonationRegistrationGetMetric>
{
    private const string OperationFieldValue = "OrganDonation_Get_Response";
    private const string DetailsFieldValue = "A default organ donation registration has been generated";

    public OrganDonationRegistrationGetMetric Parse(AuditRecord source)
    {
        if (!IsOrganDonationRegistrationGetMetric(source)) return null;
        return new OrganDonationRegistrationGetMetric
        {
            Timestamp = source.Timestamp,
            SessionId = source.SessionId,
            AuditId = source.AuditId
        };
    }

    private bool IsOrganDonationRegistrationGetMetric(AuditRecord source)
    {
        if (source == null) return false;
        return source.Operation == OperationFieldValue &&
               source.Details != null && source.Details.Contains(DetailsFieldValue);
    }
}
