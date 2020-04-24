using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Session
{
    public class VisionSessionService : ISessionService
    {
        private readonly IVisionClient _visionClient;
        private readonly ILogger<VisionSessionService> _logger;
        private readonly VisionTokenValidationService _tokenValidationService;

        public VisionSessionService(
            IVisionClient visionClient,
            ILogger<VisionSessionService> logger,
            VisionTokenValidationService tokenValidationService)
        {
            _visionClient = visionClient;
            _logger = logger;
            _tokenValidationService = tokenValidationService;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            _logger.LogEnter();
            try
            {
                if (_tokenValidationService.IsInvalidConnectionTokenFormat(connectionToken))
                {
                    _logger.LogError("Invalid Im1 connection token");
                    return new GpSessionCreateResult.InvalidConnectionToken();
                }

                var visionConnectionToken = connectionToken.DeserializeJson<VisionConnectionToken>();

                var response = await _visionClient.GetConfiguration(visionConnectionToken, odsCode);

                if (response.HasErrorResponse)
                {
                    return GetCorrectErrorResult(response);
                }

                var responseBody = response.Body;
                if(!IsResponseBodyValid(responseBody))
                {
                    const string message = "Vision HttpRequestException has been thrown.";
                    _logger.LogError(message);
                    return new GpSessionCreateResult.BadGateway(message);
                }

                return new GpSessionCreateResult.Success(
                    new VisionUserSession
                    {
                        Name = response.Body.Configuration.Account.Name,
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
                const string message = "Vision HttpRequestException has been thrown.";
                _logger.LogError(ex, message);
                return new GpSessionCreateResult.BadGateway(message);
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
        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            _logger.LogEnter();
            _logger.LogDebug("Vision user successfully deleted");
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.Success(gpUserSession));
        }

        public Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber, string patientId)
        {
            throw new System.NotImplementedException();
        }

        private GpSessionCreateResult GetCorrectErrorResult<T>(VisionPFSClient.VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                var message = $"Vision invalid request error {response.StatusCode}";
                _logger.LogError(message);
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.BadRequest(message);
            }

            if (response.IsInvalidUserCredentialsError)
            {
                var message = $"Vision invalid user credentials {response.StatusCode}";
                _logger.LogError(message);
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.Forbidden(message);
            }

            if (response.IsInvalidSecurityHeaderError)
            {
                var message = $"Vision invalid security header {response.StatusCode}";
                _logger.LogError(message);
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.InternalServerError(message);
            }

            if (response.IsUnknownError)
            {
                var message = $"Vision unknown error {response.StatusCode}";
                _logger.LogError(message);
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.BadGateway(message);
            }
            else
            {
                var message = $"Vision system is currently unavailable {response.StatusCode}";
                _logger.LogError(message);
                _logger.LogVisionErrorResponse(response);
                return new GpSessionCreateResult.BadGateway(message);
            }
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
