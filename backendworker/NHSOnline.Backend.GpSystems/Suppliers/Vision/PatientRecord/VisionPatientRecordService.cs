using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    internal class VisionPatientRecordService : IVisionPatientRecordService
    {
        private readonly ILogger<VisionPatientRecordService> _logger;
        private readonly IVisionMyRecordMapper _visionMyRecordMapper;
        private readonly PatientRecordSectionResolver _patientRecordSectionResolver;
        private readonly AllergiesSection _allergiesSection;
        private readonly MedicationsSection _medicationsSection;
        private readonly ImmunisationsSection _immunisationsSection;
        private readonly ProblemsSection _problemsSection;
        private readonly TestResultSection _testResultsSection;
        private readonly DiagnosisSection _diagnosisSection;
        private readonly ExaminationsSection _examinationsSection;
        private readonly ProceduresSection _proceduresSection;

        public VisionPatientRecordService(
            ILogger<VisionPatientRecordService> logger,
            IVisionMyRecordMapper visionMyRecordMapper,
            PatientRecordSectionResolver patientRecordSectionResolver,
            AllergiesSection allergiesSection,
            MedicationsSection medicationSection,
            ImmunisationsSection immunisationsSection,
            ProblemsSection problemsSection,
            TestResultSection testResultsSection,
            DiagnosisSection diagnosisSection,
            ExaminationsSection examinationsSection,
            ProceduresSection proceduresSection)
        {
            _logger = logger;
            _visionMyRecordMapper = visionMyRecordMapper;
            _patientRecordSectionResolver = patientRecordSectionResolver;
            _allergiesSection = allergiesSection;
            _medicationsSection = medicationSection;
            _immunisationsSection = immunisationsSection;
            _problemsSection = problemsSection;
            _testResultsSection = testResultsSection;
            _diagnosisSection = diagnosisSection;
            _examinationsSection = examinationsSection;
            _proceduresSection = proceduresSection;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            try
            {
                var userSession = gpLinkedAccountModel.GpUserSession as VisionUserSession;
                if (userSession is null)
                {
                    _logger.LogError("UserSession not Vision");
                    return new GetMyRecordResult.InternalServerError();
                }

                var allergiesTask = _patientRecordSectionResolver.GetPatientData(userSession, _allergiesSection);
                var medicationsTask = _patientRecordSectionResolver.GetPatientData(userSession, _medicationsSection);
                var immunisationsTask = _patientRecordSectionResolver.GetPatientData(userSession, _immunisationsSection);
                var problemsTask = _patientRecordSectionResolver.GetPatientData(userSession, _problemsSection);
                var testResultsTask = _patientRecordSectionResolver.GetPatientData(userSession, _testResultsSection);
                var diagnosisTask = _patientRecordSectionResolver.GetPatientData(userSession, _diagnosisSection);
                var examinationsTask = _patientRecordSectionResolver.GetPatientData(userSession, _examinationsSection);
                var proceduresTask = _patientRecordSectionResolver.GetPatientData(userSession, _proceduresSection);

                await Task.WhenAll(allergiesTask, medicationsTask, immunisationsTask, problemsTask,
                    diagnosisTask, testResultsTask, examinationsTask, proceduresTask);

                var response = _visionMyRecordMapper.Map(allergiesTask.Result, medicationsTask.Result,
                    immunisationsTask.Result, problemsTask.Result, testResultsTask.Result, diagnosisTask.Result,
                    examinationsTask.Result, proceduresTask.Result);
                response.Supplier = userSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);

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

        public async Task<GetMyRecordSectionResult> GetSection(GpUserSession gpUserSession,
            VisionRecordSectionType section)
        {
            try
            {
                var userSession = gpUserSession as VisionUserSession;
                if (userSession is null)
                {
                    _logger.LogError("UserSession not Vision");
                    return new GetMyRecordSectionResult.InternalServerError();
                }

                var result = section switch
                {
                    VisionRecordSectionType.Diagnosis =>
                    await _patientRecordSectionResolver.GetSectionResponse(userSession, _diagnosisSection),
                    VisionRecordSectionType.Examinations =>
                    await _patientRecordSectionResolver.GetSectionResponse(userSession, _examinationsSection),
                    VisionRecordSectionType.Procedures =>
                    await _patientRecordSectionResolver.GetSectionResponse(userSession, _proceduresSection),
                    VisionRecordSectionType.TestResults =>
                    await _patientRecordSectionResolver.GetSectionResponse(userSession, _testResultsSection),
                    _ => throw new ArgumentOutOfRangeException(nameof(section))
                };
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unsuccessful request retrieving {section} for Vision");
                return new GetMyRecordSectionResult.InternalServerError();
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