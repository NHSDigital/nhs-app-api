using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.TaskChecker;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<VisionPatientRecordService> _logger;
        private readonly IVisionClient _visionClient;
        private readonly IVisionConfig _config;
        private readonly IVisionMyRecordMapper _visionMyRecordMapper;

        public VisionPatientRecordService(ILogger<VisionPatientRecordService> logger, IVisionClient visionClient, IVisionConfig visionConfig, IVisionMyRecordMapper visionMyRecordMapper)
        {
            _logger = logger;
            _visionClient = visionClient;
            _config = visionConfig;
            _visionMyRecordMapper = visionMyRecordMapper;
        }

        public async Task<GetMyRecordResult> GetMyRecord(UserSession userSession)
        {
            _logger.LogEnter(nameof(GetMyRecord));
            var visionUserSession = (VisionUserSession)userSession;

            try
            {
                var request = new PatientDataRequest
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
                    }
                };

                request.ResponseFormat = ResponseFormats.HTML;
                request.View = Views.VPS_ALLERGIES;

                var allergiesTask = _visionClient.GetPatientData(visionUserSession, request);

                await Task.WhenAll(allergiesTask);
                _logger.LogInformation("Patient record tasks completed");

                try
                {
                    var checkedAllergies = new VisionGetAllergyTaskChecker(_logger).Check(allergiesTask);

                    var response = _visionMyRecordMapper.Map(checkedAllergies);
                    response.Supplier = userSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);
                    
                    _logger.LogExit(nameof(GetMyRecord));
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
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(UserSession userSession, string testResultId)
        {
            throw new NotImplementedException();
        }

        private static class ResponseFormats {
            public const string HTML = "HTML";
            public const string XML = "XML";
        }

        private static class Views {
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
    }
}