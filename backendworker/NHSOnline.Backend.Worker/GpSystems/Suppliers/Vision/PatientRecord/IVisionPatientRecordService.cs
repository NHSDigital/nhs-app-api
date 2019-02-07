using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public interface IVisionPatientRecordService : IPatientRecordService
    {
        Task<GetMyRecordSectionResult> GetSection(GpUserSession gpUserSession, VisionMapperType section);
    }
}
