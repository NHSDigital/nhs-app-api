using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    internal sealed class VisionMyRecordMapper : IVisionMyRecordMapper
    {
        public MyRecordResponse Map(VisionPatientRecordData data)
        {
            return new MyRecordResponse
            {
                Allergies = data.Allergies,
                Medications = data.Medications,
                Immunisations = data.Immunisations,
                Problems = data.Problems,
                TestResults = data.TestResults,
                Diagnosis = data.Diagnosis,
                Examinations = data.Examinations,
                Procedures = data.Procedures,
                HasSummaryRecordAccess = data.HasSummaryRecordAccess,
                HasDetailedRecordAccess = data.HasDetailedRecordAccess
            };
        }
    }
}
