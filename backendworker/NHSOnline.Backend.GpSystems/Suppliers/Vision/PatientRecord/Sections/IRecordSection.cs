using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public interface IRecordSection<T> where T : IPatientDataModel, new()
    {
        public string ViewName { get; }
        public IVisionMapper<T> Mapper { get; }
        public string FormatResponse { get; }
    }
}