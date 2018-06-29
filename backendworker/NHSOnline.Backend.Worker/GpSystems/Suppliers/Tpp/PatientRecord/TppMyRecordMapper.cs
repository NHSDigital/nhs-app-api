using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppMyRecordMapper : ITppMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, TestResults testResults,
            Problems problems)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                Immunisations = immunisations,
                TestResults = testResults,
                Problems = problems,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = immunisations.HasAccess || testResults.HasAccess
            };
        }
    }
}