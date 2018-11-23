using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<EmisPatientRecordService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisMyRecordMapper _emisMyRecordMapper;

        public EmisPatientRecordService(ILogger<EmisPatientRecordService> logger, IEmisClient emisClient, IEmisMyRecordMapper emisMyRecordMapper)
        {
            _emisClient = emisClient;
            _emisMyRecordMapper = emisMyRecordMapper;
            _logger = logger;
        }

        public async Task<GetMyRecordResult> GetMyRecord(UserSession userSession)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)userSession;

            try
            {
                _logger.LogInformation("Creating patient record api tasks");
                var medicationsTask = _emisClient.MedicalRecordGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, RecordType.Medication);

                var allergiesTask = _emisClient.MedicalRecordGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, RecordType.Allergies);

                var immunisationsTask = _emisClient.MedicalRecordGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, RecordType.Immunisations);

                var testResultsTask = _emisClient.MedicalRecordGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, RecordType.TestResults);

                var problemsTask = _emisClient.MedicalRecordGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, RecordType.Problems);

                var consultationsTask = _emisClient.MedicalRecordGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, RecordType.Consultations);

                await Task.WhenAll(allergiesTask, medicationsTask, immunisationsTask, testResultsTask, problemsTask, consultationsTask);
                _logger.LogInformation("Patient record tasks completed");

                _logger.LogInformation("Checking status of all patient record tasks");
                var allergies = new GetAllergiesTaskChecker(_logger).Check(allergiesTask);
                var medications = new GetMedicationsTaskChecker(_logger).Check(medicationsTask);
                var immunisations = new GetImmunisationsTaskChecker(_logger).Check(immunisationsTask);
                var testResults = new GetTestResultsTaskChecker(_logger).Check(testResultsTask);
                var problems = new GetProblemsTaskChecker(_logger).Check(problemsTask);
                var consultations = new GetConsultationsTaskChecker(_logger).Check(consultationsTask);

                _logger.LogInformation("Mapping EMIS responses to universal MyRecordResponse class instance");
                var myRecordResponse = _emisMyRecordMapper.Map(allergies, medications, immunisations, testResults, problems, consultations);

                myRecordResponse.Supplier = userSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);

                _logger.LogInformation("MyRecordResponse: " + myRecordResponse);

                return new GetMyRecordResult.SuccessfullyRetrieved(myRecordResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                return new GetMyRecordResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                return new GetMyRecordResult.SupplierBadData();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(UserSession userSession, string testResultId)
        {
            throw new NotImplementedException();
        }
    }
}