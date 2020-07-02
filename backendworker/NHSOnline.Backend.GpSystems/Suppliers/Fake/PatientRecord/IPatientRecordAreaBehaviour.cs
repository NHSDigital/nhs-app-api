using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord
{
    [FakeGpArea("PatientRecord")]
    public interface IPatientRecordAreaBehaviour
    {
        Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel);
        Task<GetDetailedTestResult> GetDetailedTestResult(GpLinkedAccountModel gpLinkedAccountModel,
            string testResultId);
        Task<GetPatientDocumentResult> GetPatientDocument(GpLinkedAccountModel gpLinkedAccountModel,
            string documentIdentifier, string documentType, string documentName);
        Task<GetPatientDocumentDownloadResult> GetPatientDocumentForDownload(GpLinkedAccountModel gpLinkedAccountModel,
            string documentIdentifier, string documentType, string documentName);
    }
}