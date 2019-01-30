using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionSessionService : ISessionService
    {
        private readonly IVisionClient _visionClient;
        private readonly ILogger<VisionSessionService> _logger;

        public VisionSessionService(IVisionClient visionClient, ILogger<VisionSessionService> logger)
        {
            _visionClient = visionClient;
            _logger = logger;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            _logger.LogEnter();
            try
            {
                var visionConnectionToken = connectionToken.DeserializeJson<VisionConnectionToken>();

                var response = await _visionClient.GetConfiguration(visionConnectionToken, odsCode);

                if (response.HasErrorResponse)
                {
                    return GetCorrectErrorResult(response);
                }

                var responseBody = response.Body;
                if(!IsResponseBodyValid(responseBody))
                {
                    _logger.LogError("Vision HttpRequestException has been thrown.");
                    return new GpSessionCreateResult.SupplierSystemBadResponse();
                }


                return new GpSessionCreateResult.SuccessfullyCreated(
                    response.Body.Configuration.Account.Name,
                    new VisionUserSession
                    {
                        RosuAccountId = visionConnectionToken.RosuAccountId,
                        OdsCode = odsCode,
                        ApiKey = visionConnectionToken.ApiKey,
                        NhsNumber = nhsNumber,
                        PatientId = response.Body.Configuration.Account.PatientId,
                        IsRepeatPrescriptionsEnabled = response.Body.Configuration.Prescriptions.RepeatEnabled,
                        IsAppointmentsEnabled = response.Body.Configuration.Appointments.BookingEnabled,
                        LocationIds = GetLocationIds(response)
                    }
                );
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Vision HttpRequestException has been thrown.");
                return new GpSessionCreateResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static List<string> GetLocationIds(VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse> response) =>
            response.Body.Configuration.References?.Locations != null
                ? response.Body.Configuration.References.Locations.Select(l => l.Id).ToList()
                : new List<string>();

        // Vision does not have a logoff endpoint, returning successfully deleted
        public Task<SessionLogoffResult> Logoff(UserSession userSession)
        {
            _logger.LogEnter();
            _logger.LogDebug("Vision user successfully deleted");
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.SuccessfullyDeleted(userSession));
        }

        private GpSessionCreateResult GetCorrectErrorResult<T>(VisionPFSClient.VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                _logger.LogError($"Vision invalid request error {response.StatusCode}");
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.InvalidRequest();
            }

            if (response.IsInvalidUserCredentialsError)
            {
                _logger.LogError($"Vision invalid user credentials {response.StatusCode}");
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.InvalidUserCredentials();
            }

            if (response.IsInvalidSecurityHeaderError)
            {
                _logger.LogError($"Vision invalid security header {response.StatusCode}");
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.ErrorProcessingSecurityHeader();
            }

            if (response.IsUnknownError)
            {
                _logger.LogError($"Vision unknown error {response.StatusCode}");
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.UnknownError();
            }

            _logger.LogError($"Vision system is currently unavailable {response.StatusCode}");
            _logger.LogVisionErrorResponse(response);
            
            return new GpSessionCreateResult.SupplierSystemUnavailable();
        }

        private bool IsResponseBodyValid(PatientConfigurationResponse responseBody)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(responseBody.Configuration.Account?.Name, nameof(responseBody.Configuration.Account.Name))
                .IsNotNullOrWhitespace(responseBody.Configuration.Account?.PatientId, nameof(responseBody.Configuration.Account.PatientId))
                .IsNotNull(responseBody.Configuration.Prescriptions?.RepeatEnabled, nameof(responseBody.Configuration.Prescriptions.RepeatEnabled))
                .IsNotNull(responseBody.Configuration.Appointments?.BookingEnabled, nameof(responseBody.Configuration.Appointments.BookingEnabled))
                .IsValid();
        }
    }
}
