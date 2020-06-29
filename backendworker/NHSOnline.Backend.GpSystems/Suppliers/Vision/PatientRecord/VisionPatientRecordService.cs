using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionPatientRecordService : IVisionPatientRecordService
    {
        private readonly ILogger<VisionPatientRecordService> _logger;
        private readonly IVisionClient _visionClient;
        private readonly VisionConfigurationSettings _config;
        private readonly IVisionMyRecordMapper _visionMyRecordMapper;
        private readonly VisionAllergyMapper _allergyMapper;
        private readonly VisionMedicationMapper _medicationMapper;
        private readonly VisionImmunisationsMapper _immunisationsMapper;
        private readonly VisionProblemsMapper _problemsMapper;
        private readonly VisionTestResultsMapper _testResultsMapper;
        private readonly VisionDiagnosisMapper _diagnosisMapper;
        private readonly VisionExaminationsMapper _examinationsMapper;
        private readonly VisionProceduresMapper _proceduresMapper;

        public VisionPatientRecordService(
            ILogger<VisionPatientRecordService> logger,
            IVisionClient visionClient,
            VisionConfigurationSettings visionConfig,
            IVisionMyRecordMapper visionMyRecordMapper,
            VisionAllergyMapper allergyMapper,
            VisionMedicationMapper medicationMapper,
            VisionImmunisationsMapper immunisationsMapper,
            VisionProblemsMapper problemsMapper,
            VisionTestResultsMapper testResultsMapper,
            VisionDiagnosisMapper diagnosisMapper,
            VisionExaminationsMapper examinationsMapper,
            VisionProceduresMapper proceduresMapper)
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
            _examinationsMapper = examinationsMapper;
            _proceduresMapper = proceduresMapper;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();
            var visionUserSession = (VisionUserSession)gpLinkedAccountModel.GpUserSession;

            try
            {

                var allergiesTask = RetrieveAllergies(visionUserSession);
                var medicationsTask = RetrieveMedications(visionUserSession);
                var immunisationsTask = RetrieveImmunisations(visionUserSession);
                var problemsTask = RetrieveProblems(visionUserSession);
                var testResultsTask = RetrieveTestResults(visionUserSession);
                var diagnosisTask = RetrieveDiagnosis(visionUserSession);
                var examinationsTask = RetrieveExaminations(visionUserSession);
                var proceduresTask = RetrieveProcedures(visionUserSession);

                await Task.WhenAll(allergiesTask, medicationsTask, immunisationsTask, problemsTask,
                    diagnosisTask, testResultsTask, examinationsTask, proceduresTask);

                var response = _visionMyRecordMapper.Map(allergiesTask.Result, medicationsTask.Result,
                    immunisationsTask.Result, problemsTask.Result, testResultsTask.Result, diagnosisTask.Result,
                    examinationsTask.Result, proceduresTask.Result);
                response.Supplier = visionUserSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);

                _logger.LogInformation("Patient record tasks completed");

                return new GetMyRecordResult.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong building the Vision My Record response");
                return new GetMyRecordResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetMyRecordSectionResult> GetSection(GpUserSession gpUserSession, VisionMapperType section)
        {
            var visionUserSession = (VisionUserSession)gpUserSession;

            if (section == VisionMapperType.TestResults)
            {
                return await GetTestResults(visionUserSession);
            }
            if (section == VisionMapperType.Diagnosis)
            {
                return await GetDiagnosisResults(visionUserSession);
            }
            if (section == VisionMapperType.Examinations)
            {
                return await GetExaminationResults(visionUserSession);
            }
            if (section == VisionMapperType.Procedures)
            {
                return await GetProcedureResults(visionUserSession);
            }

            return new GetMyRecordSectionResult.BadRequest();
        }

        private async Task<GetMyRecordSectionResult> GetTestResults(VisionUserSession visionUserSession)
        {
            return await GetMyRecordSection(
                visionUserSession,
                _testResultsMapper,
                VisionMapperType.TestResults,
                Views.TEST_RESULTS,
                ResponseFormats.HTML);
        }

        private async Task<GetMyRecordSectionResult> GetDiagnosisResults(VisionUserSession visionUserSession)
        {
            return await GetMyRecordSection(
                visionUserSession,
                _diagnosisMapper,
                VisionMapperType.Diagnosis,
                Views.DIAGNOSIS,
                ResponseFormats.HTML);
        }

        private async Task<GetMyRecordSectionResult> GetExaminationResults(VisionUserSession visionUserSession)
        {
            return await GetMyRecordSection(
                visionUserSession,
                _examinationsMapper,
                VisionMapperType.Examinations,
                Views.EXAM_FINDINGS,
                ResponseFormats.HTML);
        }

        private async Task<GetMyRecordSectionResult> GetProcedureResults(VisionUserSession visionUserSession)
        {
            return await GetMyRecordSection(
                visionUserSession,
                _proceduresMapper,
                VisionMapperType.Procedures,
                Views.PROCEDURES,
                ResponseFormats.HTML);
        }

        private async Task<GetMyRecordSectionResult> GetMyRecordSection<T>(VisionUserSession visionUserSession, IVisionMapper<T> mapper, VisionMapperType visionMapperType, string viewName, string responseFormat)
            where T : IVisionPatientDataModel, new()
        {
            _logger.LogEnter();

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

                    return new GetMyRecordSectionResult.Success(response);
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
                return new GetMyRecordSectionResult.BadGateway();
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

        private async Task<Allergies> RetrieveAllergies(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.HTML, Views.VPS_ALLERGIES, _allergyMapper,
                VisionMapperType.Allergies);
        }

        private async Task<Medications> RetrieveMedications(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.XML, Views.VPS_MEDICATIONS, _medicationMapper,
                VisionMapperType.Medications);
        }

        private async Task<Immunisations> RetrieveImmunisations(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.XML, Views.PROCEDURES, _immunisationsMapper,
                VisionMapperType.Immunisations);
        }
        private async Task<Problems> RetrieveProblems(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.XML, Views.PROBLEMS, _problemsMapper,
                VisionMapperType.Problems);
        }

        private async Task<TestResults> RetrieveTestResults(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.HTML, Views.TEST_RESULTS, _testResultsMapper,
                VisionMapperType.TestResults);
        }

        private async Task<Diagnosis> RetrieveDiagnosis(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.HTML, Views.DIAGNOSIS, _diagnosisMapper,
                VisionMapperType.Diagnosis);
        }

        private async Task<Examinations> RetrieveExaminations(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.HTML, Views.EXAM_FINDINGS, _examinationsMapper,
                VisionMapperType.Examinations);
        }

        private async Task<Procedures> RetrieveProcedures(VisionUserSession visionUserSession)
        {
            return await RetrieveSection(visionUserSession, ResponseFormats.HTML, Views.PROCEDURES, _proceduresMapper,
                VisionMapperType.Examinations);
        }

        private async Task<T> RetrieveSection<T>(
            VisionUserSession visionUserSession,
            string responseFormat,
            string view,
            IVisionMapper<T> mapper,
            VisionMapperType type) where T : IPatientDataModel, new()
        {
            try
            {
                var data = await _visionClient.GetPatientData(
                    visionUserSession,
                    CreatePatientDataRequest(visionUserSession, responseFormat, view));
                return new VisionTaskChecker<T>(_logger, mapper, type).Check(data);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Retrieving '{view}' section failed. Returning hasErrored as true");
                return new T() { HasErrored = true };
            }
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpLinkedAccountModel gpLinkedAccountModel, string testResultId)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientDocumentResult> GetPatientDocument(
            GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType, string documentName)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientDocumentDownloadResult> GetPatientDocumentForDownload(
            GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType, string documentName)
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
            public const string PROCEDURES = "PROCEDURES";
            public const string TEST_RESULTS = "TEST RESULTS";
            public const string EXAM_FINDINGS = "EXAM FINDINGS";
        }
    }
}