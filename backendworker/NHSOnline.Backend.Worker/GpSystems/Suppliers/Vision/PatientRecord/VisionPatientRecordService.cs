using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionPatientRecordService : IVisionPatientRecordService
    {
        private readonly ILogger<VisionPatientRecordService> _logger;
        private readonly IVisionClient _visionClient;
        private readonly IVisionPFSConfig _config;
        private readonly IVisionMyRecordMapper _visionMyRecordMapper;
        private readonly VisionAllergyMapper _allergyMapper;
        private readonly VisionMedicationMapper _medicationMapper;
        private readonly VisionImmunisationsMapper _immunisationsMapper;
        private readonly VisionProblemsMapper _problemsMapper;
        private readonly VisionTestResultsMapper _testResultsMapper;
        private readonly VisionDiagnosisMapper _diagnosisMapper;

        public VisionPatientRecordService(
            ILogger<VisionPatientRecordService> logger,
            IVisionClient visionClient,
            IVisionPFSConfig visionConfig,
            IVisionMyRecordMapper visionMyRecordMapper,
            VisionAllergyMapper allergyMapper,
            VisionMedicationMapper medicationMapper,
            VisionImmunisationsMapper immunisationsMapper,
            VisionProblemsMapper problemsMapper,
            VisionTestResultsMapper testResultsMapper,
            VisionDiagnosisMapper diagnosisMapper
        )
        {
            _logger = logger;
            _visionClient = visionClient;
            _config = visionConfig;
            _visionMyRecordMapper = visionMyRecordMapper;
            _allergyMapper = allergyMapper;
            _medicationMapper = medicationMapper;
            _immunisationsMapper = immunisationsMapper;
            _problemsMapper = problemsMapper;
            _testResultsMapper = testResultsMapper;
            _diagnosisMapper = diagnosisMapper;
        }

        public async Task<GetMyRecordResult> GetMyRecord(UserSession userSession)
        {
            _logger.LogEnter();
            var visionUserSession = (VisionUserSession) userSession.GpUserSession;

            try
            {
                var allergiesTask = _visionClient.GetPatientData(visionUserSession, CreatePatientDataRequest(visionUserSession, ResponseFormats.HTML, Views.VPS_ALLERGIES));
                var medicationsTask = _visionClient.GetPatientData(visionUserSession, CreatePatientDataRequest(visionUserSession, ResponseFormats.XML, Views.VPS_MEDICATIONS));
                var immunisationsTask = _visionClient.GetPatientData(visionUserSession, CreatePatientDataRequest(visionUserSession, ResponseFormats.XML ,Views.PROCEDURES));
                var problemsTask = _visionClient.GetPatientData(visionUserSession, CreatePatientDataRequest(visionUserSession, ResponseFormats.XML ,Views.PROBLEMS));
                var testResultsTask = _visionClient.GetPatientData(visionUserSession, CreatePatientDataRequest(visionUserSession, ResponseFormats.HTML, Views.TEST_RESULTS));
                var diagnosisTask = _visionClient.GetPatientData(visionUserSession, CreatePatientDataRequest(visionUserSession, ResponseFormats.HTML, Views.DIAGNOSIS));
                
                await Task.WhenAll(allergiesTask, medicationsTask, immunisationsTask, problemsTask, testResultsTask, diagnosisTask);
                _logger.LogInformation("Patient record tasks completed");

                try
                {
                    var checkedAllergies = new VisionTaskChecker<Allergies>(_logger, _allergyMapper, VisionMapperType.Allergies).Check(allergiesTask.Result);
                    var checkedMedications = new VisionTaskChecker<Medications>(_logger, _medicationMapper, VisionMapperType.Medications).Check(medicationsTask.Result);
                    var checkedImmunisations = new VisionTaskChecker<Immunisations>(_logger, _immunisationsMapper, VisionMapperType.Immunisations).Check(immunisationsTask.Result);
                    var checkedProblems = new VisionTaskChecker<Problems>(_logger, _problemsMapper, VisionMapperType.Problems).Check(problemsTask.Result);
                    var checkedTestResults = new VisionTaskChecker<TestResults>(_logger, _testResultsMapper, VisionMapperType.TestResults).Check(testResultsTask.Result);
                    var checkedDiagnosis = new VisionTaskChecker<Diagnosis>(_logger, _diagnosisMapper, VisionMapperType.Diagnosis).Check(diagnosisTask.Result);
                    
                    var response = _visionMyRecordMapper.Map(checkedAllergies, checkedMedications, checkedImmunisations, checkedProblems, checkedTestResults, checkedDiagnosis);
                    response.Supplier = visionUserSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);
                    
                    return new GetMyRecordResult.SuccessfullyRetrieved(response);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Something went wrong building the Vision My Record response");
                    return new GetMyRecordResult.InternalServerError();
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient selected information for Vision");
                return new GetMyRecordResult.Unsuccessful();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetMyRecordSectionResult> GetSection(UserSession userSession, VisionMapperType section)
        {
            if (section == VisionMapperType.TestResults)
            {
                return await GetTestResults(userSession);
            }
            else if (section == VisionMapperType.Diagnosis)
            {
                return await GetDiagnosisResults(userSession);
            }
            
            return new GetMyRecordSectionResult.InvalidRequest();
        }

        private async Task<GetMyRecordSectionResult> GetTestResults(UserSession userSession)
        {
            return await GetMyRecordSection<TestResults>(
                userSession,
                _testResultsMapper,
                VisionMapperType.TestResults,
                Views.TEST_RESULTS,
                ResponseFormats.HTML);
        }

        private async Task<GetMyRecordSectionResult> GetDiagnosisResults(UserSession userSession)
        {
            return await GetMyRecordSection<Diagnosis>(
                userSession,
                _diagnosisMapper,
                VisionMapperType.Diagnosis,
                Views.DIAGNOSIS,
                ResponseFormats.HTML);
        }
        
        private async Task<GetMyRecordSectionResult> GetMyRecordSection<T>(UserSession userSession, IVisionMapper<T> mapper, VisionMapperType visionMapperType, string viewName, string responseFormat) 
            where T : IVisionPatientDataModel, new()
        {
            _logger.LogEnter();
            var visionUserSession = (VisionUserSession) userSession.GpUserSession;

            try
            {
                var sectionTask = await _visionClient.GetPatientData(visionUserSession,
                    CreatePatientDataRequest(visionUserSession, responseFormat, viewName));

                _logger.LogInformation($"{viewName} task completed");

                try
                {
                    var checkedSection =
                        new VisionTaskChecker<T>(_logger, mapper, visionMapperType).Check(sectionTask);
                    var response = _visionMyRecordMapper.MapSection(checkedSection, viewName);

                    return new GetMyRecordSectionResult.SuccessfullyRetrieved(response);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Something went wrong building the Vision My Record response for {viewName}");
                    return new GetMyRecordSectionResult.InternalServerError();
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Unsuccessful request retrieving {viewName} for Vision");
                return new GetMyRecordSectionResult.Unsuccessful();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private PatientDataRequest CreatePatientDataRequest(VisionUserSession visionUserSession, string responseFormat,
            string visionView)
        {
            return new PatientDataRequest
            {
                PracticeIdentifier = visionUserSession.OdsCode,
                PatientIdentifier = visionUserSession.PatientId,
                Sender = new Sender
                {
                    Name = new SenderName
                    {
                        UserName = _config.VisionSenderUserName,
                        UserFullName = _config.VisionSenderUserFullName,
                        UserIdentity = _config.VisionSenderUserIdentity,
                        UserRole = _config.VisionSenderUserRole
                    }
                },
                ResponseFormat = responseFormat,
                View = visionView,
            };
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(UserSession userSession, string testResultId)
        {
            throw new NotImplementedException();
        }
        
        private static class ResponseFormats {
            public const string HTML = "HTML";
            public const string XML = "XML";
        }

        internal static class Views
        {
            public const string VPS_ALLERGIES = "VPS_ALLERGIES";
            public const string VPS_MEDICATIONS = "VPS_MEDICATIONS";
            public const string PROBLEMS = "PROBLEMS";
            public const string DIAGNOSIS = "DIAGNOSIS";
            public const string MEDICATIONS = "MEDICATIONS";
            public const string RISKS_AND_WARNINGS = "RISKS AND WARNINGS";
            public const string PROCEDURES = "PROCEDURES";
            public const string TEST_RESULTS = "TEST RESULTS";
            public const string EXAM_FINDINGS = "EXAM FINDINGS";
            public const string VPS_EVENT_HISTORY = "VPS_EVENT_HISTORY";
        }

        private static GetMyRecordResult GetCorrectErrorResult<T>(VisionPFSClient.VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                return new GetMyRecordResult.InvalidRequest();
            }

            if (response.IsInvalidUserCredentialsError)
            {
                return new GetMyRecordResult.InvalidUserCredentials();
            }

            if (response.IsInvalidSecurityHeaderError)
            {
                return new GetMyRecordResult.ErrorProcessingSecurityHeader();
            }

            if (response.IsUnknownError)
            {
                return new GetMyRecordResult.UnknownError();
            }

            return new GetMyRecordResult.Unsuccessful();
        }
    }
}