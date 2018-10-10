using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionMyRecordMapper : IVisionMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false
            };
        }
    }
}
