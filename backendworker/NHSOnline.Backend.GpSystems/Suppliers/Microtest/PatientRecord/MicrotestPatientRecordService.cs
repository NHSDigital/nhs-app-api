using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
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
                var medicalRecord =
                    await _microtestClient.MedicalRecordGet(microtestUserSession.OdsCode,
                        microtestUserSession.NhsNumber);
                _logger.LogInformation("Patient record tasks completed");
                
                var myRecordResponse = _microtestMyRecordMapper.Map(medicalRecord.Body);
                _logger.LogInformation(
                    "Mapped MICROTEST PatientRecordGetResponse to universal MyRecordResponse class instance");

                myRecordResponse.Supplier = microtestUserSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);
                
                return new GetMyRecordResult.Success(myRecordResponse);
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

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId)
        {
            throw new NotImplementedException();
        }
    }
}