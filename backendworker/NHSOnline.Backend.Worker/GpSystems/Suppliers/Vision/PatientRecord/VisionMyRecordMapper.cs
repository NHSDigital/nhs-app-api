using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionMyRecordMapper : IVisionMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Immunisations immunisations)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Immunisations = immunisations,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = immunisations?.HasAccess ?? false
            };
        }
    }
}
