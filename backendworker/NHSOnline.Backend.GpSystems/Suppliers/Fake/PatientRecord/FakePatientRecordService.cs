using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord
{
    public class FakePatientRecordService : FakeServiceBase, IPatientRecordService
    {
        private readonly ILogger<FakePatientRecordService> _logger;

        public FakePatientRecordService(
            ILogger<FakePatientRecordService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = FindUser(gpLinkedAccountModel);
                return await fakeUser.PatientRecordBehaviour.GetMyRecord(gpLinkedAccountModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<GetMyRecordResult>(new GetMyRecordResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpLinkedAccountModel gpLinkedAccountModel,
            string testResultId)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientDocumentResult> GetPatientDocument(GpLinkedAccountModel gpLinkedAccountModel,
            string documentIdentifier, string documentType, string documentName)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientDocumentDownloadResult> GetPatientDocumentForDownload(
            GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier,
            string documentType, string documentName)
        {
            throw new NotImplementedException();
        }
    }
}