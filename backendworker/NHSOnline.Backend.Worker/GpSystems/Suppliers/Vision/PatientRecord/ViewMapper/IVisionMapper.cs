using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public interface IVisionMapper<out T>
    {
        T Map(VisionPatientDataResponse response);
    }
}