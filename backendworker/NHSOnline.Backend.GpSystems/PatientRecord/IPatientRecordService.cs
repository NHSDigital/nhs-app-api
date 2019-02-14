using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IPatientRecordService
    {
        Task<GetMyRecordResult> GetMyRecord(GpUserSession gpUserSession);  
        
        Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId);
    }
}