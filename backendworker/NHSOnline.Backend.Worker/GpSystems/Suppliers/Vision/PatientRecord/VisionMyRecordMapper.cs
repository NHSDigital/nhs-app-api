using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionMyRecordMapper : IVisionMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Immunisations = immunisations,
                Medications = medications,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = immunisations?.HasAccess ?? false
            };
        }
    }
}
