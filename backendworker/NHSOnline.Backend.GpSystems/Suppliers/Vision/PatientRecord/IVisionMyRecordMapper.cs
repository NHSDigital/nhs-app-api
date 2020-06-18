using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    internal interface IVisionMyRecordMapper
    {
        MyRecordResponse Map(VisionPatientRecordData data);
    }
}
