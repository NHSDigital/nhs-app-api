using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMyRecordMapper : IEmisMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, TestResults testResults)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                Immunisations = immunisations,
                TestResults = testResults,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = immunisations.HasAccess || testResults.HasAccess
            };
        }
    }
}
