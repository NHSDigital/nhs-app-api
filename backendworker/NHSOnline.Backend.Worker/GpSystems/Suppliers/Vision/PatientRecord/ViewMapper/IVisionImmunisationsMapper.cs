using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public interface IVisionImmunisationsMapper
    {
        Immunisations Map(VisionPatientDataResponse response);
    }
}
