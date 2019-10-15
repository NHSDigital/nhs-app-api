using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<EmisPatientRecordService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisMyRecordMapper _emisMyRecordMapper;

        private readonly GetAllergiesTaskChecker _allergiesTaskChecker;
        private readonly GetMedicationsTaskChecker _medicationsTaskChecker;
        private readonly GetImmunisationsTaskChecker _immunistationsTaskChecker;
        private readonly GetTestResultsTaskChecker _testResultsTaskChecker;
        private readonly GetProblemsTaskChecker _problemsTaskChecker;
        private readonly GetConsultationsTaskChecker _consultationsTaskChecker;
        private readonly GetDocumentsTaskChecker _documentsTaskChecker;
        private readonly GetPatientDocumentTaskChecker _patientDocumentTaskChecker;

        public EmisPatientRecordService(
            ILogger<EmisPatientRecordService> logger,
            IEmisClient emisClient, IEmisMyRecordMapper emisMyRecordMapper,
            GetAllergiesTaskChecker allergiesTaskChecker,
            GetMedicationsTaskChecker medicationsTaskChecker, 
            GetImmunisationsTaskChecker immunisationsTaskChecker,
            GetTestResultsTaskChecker testResultsTaskChecker, 
            GetProblemsTaskChecker problemsTaskChecker,
            GetConsultationsTaskChecker consultationsTaskChecker,
            GetDocumentsTaskChecker documentsTaskChecker,
            GetPatientDocumentTaskChecker patientDocumentTaskChecker
        )
        {
            _emisClient = emisClient;
            _emisMyRecordMapper = emisMyRecordMapper;
            _logger = logger;

            _allergiesTaskChecker =allergiesTaskChecker;
            _medicationsTaskChecker = medicationsTaskChecker;
            _immunistationsTaskChecker = immunisationsTaskChecker;
            _testResultsTaskChecker = testResultsTaskChecker;
            _problemsTaskChecker = problemsTaskChecker;
            _consultationsTaskChecker = consultationsTaskChecker;
            _documentsTaskChecker = documentsTaskChecker;
            _patientDocumentTaskChecker = patientDocumentTaskChecker;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)gpUserSession;

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

                var documentsTask = _emisClient.MedicalRecordGet(emisUserSession.UserPatientLinkToken, 
                  emisUserSession.SessionId, emisUserSession.EndUserSessionId, RecordType.Documents);

                await Task.WhenAll(allergiesTask, medicationsTask, immunisationsTask, testResultsTask, problemsTask, consultationsTask, documentsTask);
                _logger.LogInformation("Patient record tasks completed");

                _logger.LogInformation("Checking status of all patient record tasks");
                var allergies = _allergiesTaskChecker.Check(allergiesTask);
                var medications = _medicationsTaskChecker.Check(medicationsTask);
                var immunisations = _immunistationsTaskChecker.Check(immunisationsTask);
                var testResults = _testResultsTaskChecker.Check(testResultsTask);
                var problems = _problemsTaskChecker.Check(problemsTask);
                var consultations = _consultationsTaskChecker.Check(consultationsTask);
                var documents = _documentsTaskChecker.Check(documentsTask);

                _logger.LogInformation("Mapping EMIS responses to universal MyRecordResponse class instance");
                var myRecordResponse = _emisMyRecordMapper.Map(allergies, medications, immunisations, testResults, problems, consultations, documents);

                myRecordResponse.Supplier = emisUserSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);

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

        public async Task<GetPatientDocumentResult> GetPatientDocument(GpUserSession gpUserSession, string documentGuid,
            string documentType, string documentName)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)gpUserSession;

            try
            {
                var getDocumentsTask = _emisClient.MedicalDocumentGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, documentGuid, emisUserSession.EndUserSessionId);

                await Task.WhenAll(getDocumentsTask);

                var documentResponse =  _patientDocumentTaskChecker.Check(getDocumentsTask, documentType, documentName);

                if (documentResponse.HasErrored)
                {
                    _logger.LogExitWith($"{nameof(documentResponse.HasErrored)}=true");
                    return new GetPatientDocumentResult.BadGateway();
                }

                return new GetPatientDocumentResult.Success(documentResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving document");
                return new GetPatientDocumentResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Record document retrieval return null body");
                return new GetPatientDocumentResult.BadGateway();
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