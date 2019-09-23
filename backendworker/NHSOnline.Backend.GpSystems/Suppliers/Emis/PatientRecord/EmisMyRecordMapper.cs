using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMyRecordMapper : IEmisMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, TestResults testResults, Problems problems, Consultations consultations, PatientDocuments documents)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                Immunisations = immunisations,
                TestResults = testResults,
                Problems = problems,
                Consultations = consultations,
                Documents = documents,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = immunisations.HasAccess || testResults.HasAccess || problems.HasAccess || consultations.HasAccess
            };
        }
    }
}
