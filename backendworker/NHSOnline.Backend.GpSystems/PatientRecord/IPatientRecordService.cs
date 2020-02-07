using NHSOnline.Backend.Support;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IPatientRecordService
    {
        Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel);

        Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId);

        Task<GetPatientDocumentResult> GetPatientDocument(GpUserSession gpUserSession, string documentIdentifier, string documentType, string documentName);
        Task<PatientDocument> GetPatientDocumentForDownload(GpUserSession gpUserSession, string documentIdentifier, string documentType, string documentName);

        byte[] ConvertDocumentToCorrectFormat(string type, string content);
    }
}