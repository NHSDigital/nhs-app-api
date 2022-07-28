using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord
{
    [FakeGpAreaBehaviour(Behaviour.Unparseable)]
    public class UnparseablePatientRecordAreaBehaviour : IPatientRecordAreaBehaviour
    {
        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var response = new MyRecordResponse
            {
                HasSummaryRecordAccess = true,
                HasDetailedRecordAccess = true,
            };
            response.Allergies.HasErrored = true;
            response.Medications.HasErrored = true;
            response.Immunisations.HasErrored = true;
            response.Problems.HasErrored = true;
            response.TestResults.HasErrored = true;
            response.HistoricTestResults.HasErrored = true;
            response.TppDcrEvents.HasErrored = true;
            response.Consultations.HasErrored = true;
            response.Diagnosis.HasErrored = true;
            response.Examinations.HasErrored = true;
            response.Procedures.HasErrored = true;
            response.Documents.HasErrored = true;

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