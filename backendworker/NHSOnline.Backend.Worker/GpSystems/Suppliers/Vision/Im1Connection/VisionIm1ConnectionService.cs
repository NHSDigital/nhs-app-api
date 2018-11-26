using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Extensions;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Im1Connection
{
    public class VisionIm1ConnectionService : IIm1ConnectionService
    {
        private readonly ILogger<VisionIm1ConnectionService> _logger;
        private readonly IVisionClient _visionClient;

        public VisionIm1ConnectionService(IVisionClient visionClient, ILogger<VisionIm1ConnectionService> logger)
        {
            _visionClient = visionClient;
            _logger = logger;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            _logger.LogEnter();

            try
            {
                var visionConnectionToken = connectionToken.DeserializeJson<VisionConnectionToken>();

                var getConfigurationReply = await _visionClient.GetConfiguration(visionConnectionToken, odsCode);

                if (getConfigurationReply.HasErrorResponse)
                {
                    _logger.LogError($"Vision system encountered an error: { getConfigurationReply.ErrorForLogging }");
                    return GetCorrectVerifyErrorResult(getConfigurationReply);
                }

                var formattedNhsNumbers = getConfigurationReply.ExtractNhsNumbers();
                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = formattedNhsNumbers
                };

                return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical("Critical Error sending configuration request to vision");
                _logger.LogCritical(ex.ToString());
                return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            _logger.LogEnter();

            try
            {
                var dob = request.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                var linkAccountRes = await _visionClient.PostLinkAccount(request.OdsCode, request, dob);
                if (linkAccountRes.HasErrorResponse)
                {
                    return GetCorrectRegisterErrorResult(linkAccountRes);
                }
                var apiToken = linkAccountRes.Body.AuthenticationRef.ApiToken;
                var visionConnectionToken = new VisionConnectionToken
                {
                    RosuAccountId = request.AccountId,
                    ApiKey = apiToken,
                };
                var configResponse = await _visionClient.GetConfiguration(visionConnectionToken, request.OdsCode);
                if (configResponse.HasErrorResponse)
                {
                    _logger.LogError("Error occurred when trying to obtain nhs number from get configuration. " +
                             $"Error Code: {configResponse.RawResponse.Body.VisionResponse.ServiceHeader.Outcome.Error.Code}. " + 
                             $"Error description: {configResponse.RawResponse.Body.VisionResponse.ServiceHeader.Outcome.Error.Description}.");
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = configResponse.ExtractNhsNumbers();
                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = visionConnectionToken.SerializeJson(),
                    NhsNumbers = nhsNumbers
                };
                 _logger.LogDebug("VisionIm1ConnectionService Register successfully completed");
                return new Im1ConnectionRegisterResult.SuccessfullyRegistered(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogCritical("Critical Error sending register request to vision");
                _logger.LogCritical(e.ToString());
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private Im1ConnectionVerifyResult GetCorrectVerifyErrorResult<T>(VisionClient.VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                LogError<T>("Invalid Request");
                return new Im1ConnectionVerifyResult.InvalidRequest();
            }

            if (response.IsInvalidUserCredentialsError)
            {
                LogError<T>("Invalid User Credentials");
                return new Im1ConnectionVerifyResult.InvalidUserCredentials();
            }

            if (response.IsInvalidSecurityHeaderError)
            {
                LogError<T>("Invalid Security Error");
                return new Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader();
            }

            if (response.IsUnknownError)
            {
                LogError<T>("Unknown Error");
                return new Im1ConnectionVerifyResult.UnknownError();
            }
            LogError<T>("Other Error");
            return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
        }
        
        private Im1ConnectionRegisterResult GetCorrectRegisterErrorResult<T>(VisionClient.VisionApiObjectResponse<T> response)
        {
            if (response.IsAccountLockedError)
            {
                LogError<T>("User account locked");
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }
            
            if (response.IsAlreadyRegisteredError)
            {
                LogError<T>("User already registered");
                return new Im1ConnectionRegisterResult.AccountAlreadyExists();
            }
            
            if (response.IsInvalidDetailsError)
            {
                LogError<T>("Invalid supplied details");
                return new Im1ConnectionRegisterResult.NotFound();
            }
            
            if (response.IsInvalidParameterError)
            {
                LogError<T>("Invalid supplied details");
                return new Im1ConnectionRegisterResult.BadRequest();
            }

            if (response.IsUnknownError)
            {
                LogError<T>("Unknown error");
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }

            LogError<T>("Other Error");
            return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
       }
        
        private void LogError<T>(string errorType)
        {
            _logger.LogError("Vision Im1Connection error of type '" + errorType + "'. ");
        }
    }
}