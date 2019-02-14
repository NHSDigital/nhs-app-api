using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public interface IVisionMapper<out T>
    {
        T Map(VisionPatientDataResponse response);
    }
}