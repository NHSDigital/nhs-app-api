
namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Create
{
    public class NominatedPharmacyCreateEventParser : IAuditLogParser<NominatedPharmacyCreateMetric>
    {
        private const string OperationFieldValue = "NominatedPharmacy_Update_Response";
        private const string DetailsFieldValue = "Successfully created new nominated pharmacy registration";

        public NominatedPharmacyCreateMetric Parse(AuditRecord source)
        {
            if (!IsNominatedPharmacyCreateMetric(source)) return null;

            return new NominatedPharmacyCreateMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsNominatedPharmacyCreateMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}
