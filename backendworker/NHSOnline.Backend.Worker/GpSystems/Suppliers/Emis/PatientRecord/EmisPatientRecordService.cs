using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisPatientRecordService : IPatientRecordService
    {
        private readonly ILogger _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisMyRecordMapper _emisMyRecordMapper;

        public EmisPatientRecordService(ILoggerFactory loggerFactory, IEmisClient emisClient, IEmisMyRecordMapper emisMyRecordMapper)
        {
            _emisClient = emisClient;
            _emisMyRecordMapper = emisMyRecordMapper;
            _logger = loggerFactory.CreateLogger<EmisPatientRecordService>();
        }
        
        public async Task<GetMyRecordResult> Get(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
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

                var allergies = new GetAllergiesTaskChecker(_logger).Check(allergiesTask);
                var medications = new GetMedicationsTaskChecker(_logger).Check(medicationsTask);
                var immunisations = new GetImmunisationsTaskChecker(_logger).Check(immunisationsTask);
                var testResults = new GetTestResultsTaskChecker(_logger).Check(testResultsTask);
                var problems = new GetProblemsTaskChecker(_logger).Check(problemsTask);
                var consultations = new GetConsultationsTaskChecker(_logger).Check(consultationsTask);
                
                var myRecordResponse = _emisMyRecordMapper.Map(allergies, medications, immunisations, testResults, problems, consultations);

                myRecordResponse.Supplier = userSession.Supplier.ToString().ToUpper();

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
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(UserSession userSession, string testResultId)
        {
            throw new NotImplementedException();
        }
    }
}