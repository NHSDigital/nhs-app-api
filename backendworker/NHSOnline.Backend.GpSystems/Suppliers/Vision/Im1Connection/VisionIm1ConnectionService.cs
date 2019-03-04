using System.Globalization;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection
{
    public class VisionIm1ConnectionService : IIm1ConnectionService
    {
        private readonly IVisionClient _visionClient;
        private readonly IIm1CacheService _im1CacheService;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly ILogger<VisionIm1ConnectionService> _logger;

        public VisionIm1ConnectionService(
            IVisionClient visionClient,
            IIm1CacheService im1CacheService,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            ILogger<VisionIm1ConnectionService> logger)
        {
            _visionClient = visionClient;
            _im1CacheService = im1CacheService;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
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

                var formattedNhsNumbers = getConfigurationReply.Body.Configuration.ExtractNhsNumbers();
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
                _logger.LogDebug("Checking cache for IM1 connection token");
                var key = _im1CacheKeyGenerator.GenerateCacheKey(
                    request.AccountId, request.OdsCode, request.LinkageKey);
                var cachedConnectionToken =
                    await _im1CacheService.GetIm1ConnectionToken<VisionConnectionToken>(key);

                VisionConnectionToken connectionToken;

                if (cachedConnectionToken.HasValue)
                {
                    connectionToken = cachedConnectionToken.ValueOrFailure();
                    _logger.LogDebug("IM1 connection token found in cache.");
                }
                else
                {
                    var dob = request.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    
                    VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse> linkAccountRes;
                    
                    try
                    {
                        linkAccountRes = await _visionClient.PostLinkAccount(request.OdsCode, request, dob);
                    }
                    catch (SocketException ex)
                    {
                        _logger.LogError(ex, $"Vision user with AccountId:{request.AccountId} throwing a Socket Exception.  Possibly already linked." + ex.ToString() );
                        return new Im1ConnectionRegisterResult.AccountAlreadyExists();
                    }

                    if (linkAccountRes.HasErrorResponse)
                    {
                        return GetCorrectRegisterErrorResult(linkAccountRes);
                    }
                    var apiToken = linkAccountRes.Body.AuthenticationRef.ApiToken;
                    connectionToken = new VisionConnectionToken
                    {
                        RosuAccountId = request.AccountId,
                        ApiKey = apiToken,
                        Im1CacheKey = key,
                    };

                    await _im1CacheService.SaveIm1ConnectionToken(key, connectionToken);
                }

                var configResponse = await _visionClient.GetConfiguration(connectionToken, request.OdsCode);
                if (configResponse.HasErrorResponse)
                {
                    _logger.LogError("Error occurred when trying to obtain nhs number from get configuration. " +
                             $"Error Code: {configResponse.RawResponse.Body.VisionResponse.ServiceHeader.Outcome.Error.Code}. " +
                             $"Error description: {configResponse.RawResponse.Body.VisionResponse.ServiceHeader.Outcome.Error.Description}.");
                    _logger.LogVisionErrorResponse(configResponse);
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = configResponse.Body.Configuration.ExtractNhsNumbers();
                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken.SerializeJson(),
                    NhsNumbers = nhsNumbers
                };
                _logger.LogDebug($"{nameof(VisionIm1ConnectionService)} Register successfully completed");

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

        private Im1ConnectionVerifyResult GetCorrectVerifyErrorResult<T>(VisionPFSClient.VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                LogError<T>("Invalid Request");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionVerifyResult.InvalidRequest();
            }

            if (response.IsInvalidUserCredentialsError)
            {
                LogError<T>("Invalid User Credentials");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionVerifyResult.InvalidUserCredentials();
            }

            if (response.IsInvalidSecurityHeaderError)
            {
                LogError<T>("Invalid Security Error");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader();
            }

            if (response.IsUnknownError)
            {
                LogError<T>("Unknown Error");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionVerifyResult.UnknownError();
            }
            LogError<T>("Other Error");
            _logger.LogVisionErrorResponse(response);
            return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
        }
        
        private Im1ConnectionRegisterResult GetCorrectRegisterErrorResult<T>(VisionPFSClient.VisionApiObjectResponse<T> response)
        {
            if (response.IsAccountLockedError)
            {
                LogError<T>("User account locked");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }
            
            if (response.IsAlreadyRegisteredError)
            {
                LogError<T>("User already registered");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionRegisterResult.AccountAlreadyExists();
            }
            
            if (response.IsInvalidDetailsError)
            {
                LogError<T>("Invalid supplied details");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionRegisterResult.NotFound();
            }
            
            if (response.IsInvalidParameterError)
            {
                LogError<T>("Invalid supplied details");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionRegisterResult.BadRequest();
            }

            if (response.IsUnknownError)
            {
                LogError<T>("Unknown error");
                _logger.LogVisionErrorResponse(response);
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }

            LogError<T>("Other Error");
            _logger.LogVisionErrorResponse(response);
            return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
       }
        
        private void LogError<T>(string errorType)
        {
            _logger.LogError("Vision Im1Connection error of type '" + errorType + "'. ");
        }
    }
}
