using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public interface IVisionMyRecordMapper
    {
        MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, Problems problems, TestResults testResults);
    }
}
