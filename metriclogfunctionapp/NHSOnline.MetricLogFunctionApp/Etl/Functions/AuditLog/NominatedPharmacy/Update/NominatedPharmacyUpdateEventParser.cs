
namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Update
{
    public class NominatedPharmacyUpdateEventParser : IAuditLogParser<NominatedPharmacyUpdateMetric>
    {
        private const string OperationFieldValue = "NominatedPharmacy_Update_Response";
        private const string DetailsFieldValue = "Successfully updated nominated pharmacy";

        public NominatedPharmacyUpdateMetric Parse(AuditRecord source)
        {
            if (!IsNominatedPharmacyUpdateMetric(source)) return null;

            return new NominatedPharmacyUpdateMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsNominatedPharmacyUpdateMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}
