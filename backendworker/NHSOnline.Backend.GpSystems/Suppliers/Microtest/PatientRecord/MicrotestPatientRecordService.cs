using System;
using System.Globalization;
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
    public class MicrotestPatientRecordService : IPatientRecordService
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

        public async Task<GetMyRecordResult> GetMyRecord(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var microtestUserSession = (MicrotestUserSession) gpUserSession;

            try
            {
                var medicalRecordResponse =
                    await _microtestClient.MedicalRecordGet(microtestUserSession.OdsCode,
                        microtestUserSession.NhsNumber);

                if (medicalRecordResponse.HasSuccessResponse)
                {
                    return HandleSuccess(microtestUserSession, medicalRecordResponse);
                }
                else if (medicalRecordResponse.HasForbiddenResponse)
                {
                    _logger.LogError("The Microtest patient record service is not enabled");
                    return new GetMyRecordResult.Success(new MyRecordResponse());
                }
                else
                {
                    _logger.LogError("Microtest system is currently unavailable");
                    return new GetMyRecordResult.BadGateway();
                }
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

        private GetMyRecordResult HandleSuccess(MicrotestUserSession microtestUserSession,
            MicrotestClient.MicrotestApiObjectResponse<PatientRecordGetResponse> medicalRecordResponse )
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

    }
}