using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionMyRecordMapper : IVisionMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, Problems problems)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                Immunisations = immunisations,
                Problems = problems,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = immunisations?.HasAccess ?? problems?.HasAccess ?? false
            };
        }
    }
}
