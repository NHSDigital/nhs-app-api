using System.Globalization;
using System.Linq;
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
using Im1ConnectionErrorCodes = NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

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
                    _logger.LogError($"Vision system encountered an error: {getConfigurationReply.ErrorForLogging}");
                    return VisionIm1VerifyErrorMapper.Map(getConfigurationReply, _logger);
                }

                var formattedNhsNumbers = getConfigurationReply.Body.Configuration.ExtractNhsNumbers();
                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = formattedNhsNumbers,
                    OdsCode = odsCode
                };

                _logger.LogInformation($"Vision returned {response.NhsNumbers?.Count()} NHS Numbers for the user");

                return new Im1ConnectionVerifyResult.Success(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical("Critical Error sending configuration request to vision");
                _logger.LogCritical(ex.ToString());
                return new Im1ConnectionVerifyResult.BadGateway();
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
                var getOrCreateIm1ConnectionTokenStep = await GetOrCreateIm1ConnectionToken(request);
                if (getOrCreateIm1ConnectionTokenStep.ProcessFinishedEarly(out var getOrCreateFinalResult))
                {
                    return getOrCreateFinalResult;
                }

                var connectionToken = getOrCreateIm1ConnectionTokenStep.Result;

                var getConfigurationStep = await GetConfiguration(connectionToken, request.OdsCode);
                if (getConfigurationStep.ProcessFinishedEarly(out var getConfigurationStepFinalResult))
                {
                    return getConfigurationStepFinalResult;
                }

                var configResponse = getConfigurationStep.Result;

                var response = CreatePatientIm1ConnectionResponse(request, configResponse, connectionToken);

                _logger.LogInformation($"Vision returned {response.NhsNumbers?.Count()} NHS Numbers for the user");
                _logger.LogDebug($"{nameof(VisionIm1ConnectionService)} Register successfully completed");

                return new Im1ConnectionRegisterResult.Success(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogCritical(e, "Critical Error sending register request to vision");
                return new Im1ConnectionRegisterResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<ProcessResult<VisionConnectionToken, Im1ConnectionRegisterResult>> GetOrCreateIm1ConnectionToken(
            PatientIm1ConnectionRequest request)
        {
            var getCachedStep = await GetCachedIm1ConnectionToken(request);
            if (getCachedStep.ProcessFinishedEarly(out var cachedConnectionToken))
            {
                return ProcessResult.StepResult<VisionConnectionToken, Im1ConnectionRegisterResult>(
                    cachedConnectionToken);
            }

            var cacheKey = getCachedStep.Result;

            return await CreateIm1ConnectionToken(request, cacheKey);
        }

        private async Task<ProcessResult<string, VisionConnectionToken>> GetCachedIm1ConnectionToken(
            PatientIm1ConnectionRequest request)
        {
            var cacheKey =
                _im1CacheKeyGenerator.GenerateCacheKey(request.AccountId, request.OdsCode, request.LinkageKey);

            _logger.LogDebug("Checking cache for IM1 connection token");
            var cachedConnectionToken = await _im1CacheService.GetIm1ConnectionToken<VisionConnectionToken>(cacheKey);

            return await cachedConnectionToken
                .IfSome(connectionToken =>
                {
                    _logger.LogDebug("IM1 connection token found in cache.");
                    var finalResult = ProcessResult.FinalResult<string, VisionConnectionToken>(connectionToken);
                    return Task.FromResult(finalResult);
                })
                .IfNone(() =>
                {
                    var stepResult = ProcessResult.StepResult<string, VisionConnectionToken>(cacheKey);
                    return Task.FromResult(stepResult);
                });
        }

        private async Task<ProcessResult<VisionConnectionToken, Im1ConnectionRegisterResult>> CreateIm1ConnectionToken(
            PatientIm1ConnectionRequest request,
            string cacheKey)
        {
            var dob = request.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            VisionPfsApiObjectResponse<ServiceContentRegisterResponse> linkAccountResponse;

            try
            {
                linkAccountResponse = await _visionClient.PostLinkAccount(request.OdsCode, request, dob);
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex,
                    $"Vision user with AccountId:{request.AccountId} throwing a Socket Exception.  Possibly already linked.");
                var finalResult =
                    new Im1ConnectionRegisterResult.ErrorCase(
                        Im1ConnectionErrorCodes.InternalCode.UserAlreadyLinked);
                return ProcessResult.FinalResult<VisionConnectionToken, Im1ConnectionRegisterResult>(finalResult);
            }

            if (linkAccountResponse.HasErrorResponse)
            {
                var finalResult = VisionIm1RegisterErrorMapper.Map(linkAccountResponse, _logger);
                return ProcessResult.FinalResult<VisionConnectionToken, Im1ConnectionRegisterResult>(finalResult);
            }

            var apiToken = linkAccountResponse.Body.AuthenticationRef.ApiToken;

            var connectionToken = new VisionConnectionToken
            {
                RosuAccountId = request.AccountId,
                ApiKey = apiToken,
                Im1CacheKey = cacheKey,
            };

            await _im1CacheService.SaveIm1ConnectionToken(cacheKey, connectionToken);

            return ProcessResult.StepResult<VisionConnectionToken, Im1ConnectionRegisterResult>(connectionToken);
        }

        private async Task<ProcessResult<VisionPfsApiObjectResponse<PatientConfigurationResponse>, Im1ConnectionRegisterResult>> GetConfiguration(
            VisionConnectionToken connectionToken,
            string odsCode)
        {
            var configResponse = await _visionClient.GetConfiguration(connectionToken, odsCode);
            if (configResponse.HasErrorResponse)
            {
                if (configResponse.FaultExists)
                {
                    _logger.LogError("Error occurred when trying to obtain nhs number from get configuration. " +
                                     $"Error: {configResponse.ErrorForLogging}.");
                }
                else
                {
                    _logger.LogError("Error occurred when trying to obtain nhs number from get configuration. " +
                                     $"Error Code: {configResponse.RawResponse.Body.VisionResponse.ServiceHeader.Outcome.Error.Code}. " +
                                     $"Error description: {configResponse.RawResponse.Body.VisionResponse.ServiceHeader.Outcome.Error.Description}.");
                    _logger.LogVisionErrorResponse(configResponse);
                }

                var finalResult = new Im1ConnectionRegisterResult.BadGateway();
                return ProcessResult.FinalResult<VisionPfsApiObjectResponse<PatientConfigurationResponse>, Im1ConnectionRegisterResult>(finalResult);
            }

            return ProcessResult.StepResult<VisionPfsApiObjectResponse<PatientConfigurationResponse>, Im1ConnectionRegisterResult>(configResponse);
        }

        private CreateIm1ConnectionResponse CreatePatientIm1ConnectionResponse(
            PatientIm1ConnectionRequest request,
            VisionPfsApiObjectResponse<PatientConfigurationResponse> configResponse,
            VisionConnectionToken connectionToken)
        {
            var nhsNumbers = configResponse.Body.Configuration.ExtractNhsNumbers();
            return new CreateIm1ConnectionResponse
            {
                ConnectionToken = connectionToken.SerializeJson(),
                NhsNumbers = nhsNumbers,
                OdsCode = request.OdsCode,
                AccountId = request.AccountId,
                LinkageKey = request.LinkageKey
            };
        }
    }
}