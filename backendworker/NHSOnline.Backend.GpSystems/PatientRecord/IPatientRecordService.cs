using NHSOnline.Backend.Support;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IPatientRecordService
    {
        Task<GetMyRecordResult> GetMyRecord(GpUserSession gpUserSession);  
        
        Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId);

        Task<GetPatientDocumentResult> GetPatientDocument(GpUserSession gpUserSession, string documentGuid);
    }
}