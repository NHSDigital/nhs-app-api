using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public interface IPatientRecordService
    {
        Task<GetMyRecordResult> GetMyRecord(UserSession userSession);  
        
        Task<GetDetailedTestResult> GetDetailedTestResult(UserSession userSession, string testResultId);
    }
}