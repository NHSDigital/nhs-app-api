using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord
{
    [FakeGpAreaBehaviour(Behaviour.Unauthorised)]
    public class UnauthorisedPatientRecordAreaBehaviour : IPatientRecordAreaBehaviour
    {
        public Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpLinkedAccountModel gpLinkedAccountModel, string testResultId)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetPatientDocumentResult> GetPatientDocument(GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType,
            string documentName)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetPatientDocumentDownloadResult> GetPatientDocumentForDownload(GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier,
            string documentType, string documentName)
        {
            throw new System.NotImplementedException();
        }
    }
}