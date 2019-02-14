using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public interface IVisionPatientRecordService : IPatientRecordService
    {
        Task<GetMyRecordSectionResult> GetSection(GpUserSession gpUserSession, VisionMapperType section);
    }
}
