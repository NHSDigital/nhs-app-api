using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public interface IVisionMyRecordMapper
    {
        MyRecordResponse Map(PatientConfiguration patientConfiguration);
    }
}
