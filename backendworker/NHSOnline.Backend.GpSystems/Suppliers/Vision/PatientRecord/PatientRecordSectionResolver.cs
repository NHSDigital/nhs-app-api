using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    internal class PatientRecordSectionResolver
    {
        private readonly ILogger<PatientRecordSectionResolver> _logger;
        private readonly IVisionClient _visionClient;
        private readonly VisionConfigurationSettings _config;
        private readonly IVisionMyRecordMapper _visionMyRecordSectionMapper;

        public PatientRecordSectionResolver(
            ILogger<PatientRecordSectionResolver> logger,
            IVisionClient visionClient,
            VisionConfigurationSettings visionConfig,
            IVisionMyRecordMapper visionMyRecordSectionMapper)
        {
            _logger = logger;
            _visionClient = visionClient;
            _config = visionConfig;
            _visionMyRecordSectionMapper = visionMyRecordSectionMapper;
        }

        public async Task<T> GetPatientData<T>(
            VisionUserSession visionUserSession,
            string responseFormat,
            string view,
            IVisionMapper<T> mapper) where T : IPatientDataModel, new()
        {
            try
            {
                return await GetVisionPatientData(visionUserSession, responseFormat, view, mapper);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Vision Record Section '{view}': Failure");
                return new T { HasErrored = true };
            }
        }

        public async Task<GetMyRecordSectionResult> GetSectionResponse<T>(
            VisionUserSession visionUserSession,
            string responseFormat,
            string view,
            IVisionMapper<T> mapper) where T : IVisionPatientDataModel, new()
        {
            try
            {
                var response = await GetVisionPatientData(visionUserSession, responseFormat, view, mapper);
                var mappedResponse = _visionMyRecordSectionMapper.MapSection(response, view);
                return new GetMyRecordSectionResult.Success(mappedResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unsuccessful request retrieving {view} for Vision");
                return new GetMyRecordSectionResult.BadGateway();
            }
        }

        private async Task<T> GetVisionPatientData<T>(
            VisionUserSession visionUserSession,
            string responseFormat,
            string view,
            IVisionMapper<T> mapper)
            where T : IPatientDataModel, new()
        {
            var request = CreatePatientDataRequest(visionUserSession, responseFormat, view);
            var sectionData = await _visionClient.GetPatientData(visionUserSession, request);
            return Check(sectionData, view, mapper);
        }

        private T Check<T>(VisionPfsApiObjectResponse<VisionPatientDataResponse> sectionData,
            string view,
            IVisionMapper<T> mapper)
            where T : IPatientDataModel, new()
        {
            var logPrefix = $"Vision Record Section '{view}': ";
            _logger.LogInformation(logPrefix + "Checking");

            if (!sectionData.HasErrorResponse)
            {
                _logger.LogInformation(logPrefix + "Successfully checked");
                return mapper.Map(sectionData.Body);
            }

            if (sectionData.IsAccessDeniedError)
            {
                _logger.LogWarning(logPrefix + "User does not have access");
                return new T { HasAccess = false };
            }

            _logger.LogError(logPrefix + $"Unsuccessful request. Status code: {(int)sectionData.StatusCode}");
            return new T { HasErrored = true };
        }

        private PatientDataRequest CreatePatientDataRequest(
            VisionUserSession visionUserSession,
            string responseFormat,
            string visionView)
        {
            return new PatientDataRequest
            {
                PracticeIdentifier = visionUserSession.OdsCode,
                PatientIdentifier = visionUserSession.PatientId,
                Sender = CreateSender(),
                ResponseFormat = responseFormat,
                View = visionView
            };
        }

        private Sender CreateSender()
        {
            return new Sender
            {
                Name = new SenderName
                {
                    UserName = _config.VisionSenderUserName,
                    UserFullName = _config.VisionSenderUserFullName,
                    UserIdentity = _config.VisionSenderUserIdentity,
                    UserRole = _config.VisionSenderUserRole
                }
            };
        }
    }
}