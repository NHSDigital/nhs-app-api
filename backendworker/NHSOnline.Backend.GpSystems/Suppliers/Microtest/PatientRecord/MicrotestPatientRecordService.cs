using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<MicrotestPatientRecordService> _logger;
        private readonly IMicrotestClient _microtestClient;
        private readonly IMicrotestMyRecordMapper _microtestMyRecordMapper;

        public MicrotestPatientRecordService(
            ILogger<MicrotestPatientRecordService> logger,
            IMicrotestClient microtestClient,
            IMicrotestMyRecordMapper microtestMyRecordMapper
        )
        {
            _microtestClient = microtestClient;
            _logger = logger;
            _microtestMyRecordMapper = microtestMyRecordMapper;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var microtestUserSession = (MicrotestUserSession)gpLinkedAccountModel.GpUserSession;

            try
            {

                return await RetrieveMicrotestPatientRecord(microtestUserSession);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                return new GetMyRecordResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                return new GetMyRecordResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private GetMyRecordResult HandleSuccess(
            MicrotestUserSession microtestUserSession,
            MicrotestClient.MicrotestApiObjectResponse<PatientRecordGetResponse> medicalRecordResponse)
        {
            _logger.LogInformation("Patient record tasks completed");

            var myRecordResponse = _microtestMyRecordMapper.Map(medicalRecordResponse.Body);
            _logger.LogInformation(
                "Mapped Microtest PatientRecordGetResponse to universal MyRecordResponse class instance");

            myRecordResponse.Supplier =
                microtestUserSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);

            return new GetMyRecordResult.Success(myRecordResponse);
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId)
        {
            throw new NotImplementedException();
        }

        public Task<PatientDocument> GetPatientDocumentForDownload(
            GpUserSession gpUserSession,
            string documentIdentifier,
            string documentType,
            string documentName)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientDocumentResult> GetPatientDocument(
            GpUserSession gpUserSession,
            string documentIdentifier,
            string documentType,
            string documentName)
        {
            throw new NotImplementedException();
        }
        public byte[] ConvertDocumentToCorrectFormat(string type, string content)
        {
            throw new NotImplementedException();
        }

        private async Task<GetMyRecordResult> RetrieveMicrotestPatientRecord(MicrotestUserSession microtestUserSession)
        {
            var medicalRecordResponse = _microtestClient.MedicalRecordGet(microtestUserSession.OdsCode,
                    microtestUserSession.NhsNumber);

            await medicalRecordResponse;

            if (medicalRecordResponse.Result.HasSuccessResponse)
            {
                return HandleSuccess(microtestUserSession, medicalRecordResponse.Result);
            }

            if (medicalRecordResponse.Result.HasForbiddenResponse)
            {
                _logger.LogError("The Microtest patient record service is not enabled");
                return new GetMyRecordResult.Success(new MyRecordResponse());
            }

            _logger.LogError("Microtest system is currently unavailable");
            return HandleSuccess(microtestUserSession, GetEmptyMicrotestMedicalRecordResponse());
        }

        private static MicrotestClient.MicrotestApiObjectResponse<PatientRecordGetResponse> GetEmptyMicrotestMedicalRecordResponse() =>
            new MicrotestClient.MicrotestApiObjectResponse<PatientRecordGetResponse>(HttpStatusCode.OK)
            {
                Body = new PatientRecordGetResponse
                {
                    AllergyData = { HasErrored = true },
                    MedicationData = { HasErrored = true },
                    ImmunisationData = { HasErrored = true },
                    ProblemData = { HasErrored = true },
                    TestResultData = { HasErrored = true },
                    RecallData = { HasErrored = true },
                    EncounterData = { HasErrored = true },
                    ReferralData = { HasErrored = true },
                    MedicalHistoryData = { HasErrored = true }
                }
            };
    }
}