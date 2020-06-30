using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    internal class VisionPatientRecordService : IVisionPatientRecordService
    {
        private readonly ILogger<VisionPatientRecordService> _logger;
        private readonly PatientRecordSectionResolver _recordSectionResolver;
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
            PatientRecordSectionResolver recordSectionResolver,
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
            _recordSectionResolver = recordSectionResolver;
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
            var visionUserSession = (VisionUserSession) gpLinkedAccountModel.GpUserSession;

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
            var visionUserSession = (VisionUserSession) gpUserSession;

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
            return await _recordSectionResolver.GetSectionResponse<TestResults>(visionUserSession, ResponseFormats.HTML,
                Views.TEST_RESULTS,
                _testResultsMapper);
        }

        private async Task<GetMyRecordSectionResult> GetDiagnosisResults(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetSectionResponse<Diagnosis>(visionUserSession, ResponseFormats.HTML,
                Views.DIAGNOSIS,
                _diagnosisMapper);
        }

        private async Task<GetMyRecordSectionResult> GetExaminationResults(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetSectionResponse<Examinations>(visionUserSession,
                ResponseFormats.HTML, Views.EXAM_FINDINGS,
                _examinationsMapper);
        }

        private async Task<GetMyRecordSectionResult> GetProcedureResults(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetSectionResponse<Procedures>(visionUserSession, ResponseFormats.HTML,
                Views.PROCEDURES,
                _proceduresMapper);
        }

        private async Task<Allergies> RetrieveAllergies(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.HTML,
                Views.VPS_ALLERGIES, _allergyMapper);
        }

        private async Task<Medications> RetrieveMedications(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.XML,
                Views.VPS_MEDICATIONS, _medicationMapper);
        }

        private async Task<Immunisations> RetrieveImmunisations(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.XML, Views.PROCEDURES,
                _immunisationsMapper);
        }

        private async Task<Problems> RetrieveProblems(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.XML, Views.PROBLEMS,
                _problemsMapper);
        }

        private async Task<TestResults> RetrieveTestResults(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.HTML,
                Views.TEST_RESULTS, _testResultsMapper);
        }

        private async Task<Diagnosis> RetrieveDiagnosis(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.HTML, Views.DIAGNOSIS,
                _diagnosisMapper);
        }

        private async Task<Examinations> RetrieveExaminations(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.HTML,
                Views.EXAM_FINDINGS, _examinationsMapper);
        }

        private async Task<Procedures> RetrieveProcedures(VisionUserSession visionUserSession)
        {
            return await _recordSectionResolver.GetPatientData(visionUserSession, ResponseFormats.HTML,
                Views.PROCEDURES, _proceduresMapper);
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpLinkedAccountModel gpLinkedAccountModel,
            string testResultId)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientDocumentResult> GetPatientDocument(
            GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType,
            string documentName)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientDocumentDownloadResult> GetPatientDocumentForDownload(
            GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType,
            string documentName)
        {
            throw new NotImplementedException();
        }
    }
}