using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IPatientRecordService
    {
        Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel);
        Task<GetDetailedTestResult> GetDetailedTestResult(GpLinkedAccountModel gpLinkedAccountModel, string testResultId);
        Task<GetPatientDocumentResult> GetPatientDocument(GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType, string documentName);
        Task<GetPatientDocumentDownloadResult> GetPatientDocumentForDownload(GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType, string documentName);
    }
}