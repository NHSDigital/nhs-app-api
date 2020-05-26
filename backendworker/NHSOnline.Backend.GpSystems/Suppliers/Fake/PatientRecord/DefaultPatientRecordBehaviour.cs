using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord
{
    public class DefaultPatientRecordBehaviour : IPatientRecordBehaviour
    {
        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var response = new MyRecordResponse();
            return await Task.FromResult<GetMyRecordResult>(new GetMyRecordResult.Success(response));
        }

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