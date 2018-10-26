using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMyRecordMapper : IEmisMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, TestResults testResults, Problems problems, Consultations consultations)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                Immunisations = immunisations,
                TestResults = testResults,
                Problems = problems,
                Consultations = consultations,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = immunisations.HasAccess || testResults.HasAccess || problems.HasAccess || consultations.HasAccess
            };
        }
    }
}
