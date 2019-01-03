using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Im1Connection
{
    public class EmisIm1ConnectionService : IIm1ConnectionService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisIm1ConnectionService> _logger;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;

        public EmisIm1ConnectionService(IEmisClient emisClient, 
            ILogger<EmisIm1ConnectionService> logger,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService)
        {
            _emisClient = emisClient;
            _logger = logger;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            try
            {
                _logger.LogEnter();

                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessResponse)
                {
                    LogExceptionError(nameof(_emisClient.SessionsEndUserSessionPost), endUserSessionResponse);
                    _logger.LogEmisErrorResponse(endUserSessionResponse);
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;

                var either = EmisConnectionTokenParser.Parse(connectionToken);

                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = either.Match(guid => guid, ct => ct.AccessIdentityGuid),
                    NationalPracticeCode = odsCode
                };

                var sessionsResponse = await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessResponse)
                {
                    LogExceptionError(nameof(_emisClient.SessionsPost), sessionsResponse);
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var userPatientLinkToken = sessionsResponse.Body?.ExtractUserPatientLinkToken();
                if (string.IsNullOrEmpty(userPatientLinkToken))
                {
                    _logger.LogError($"Emis {nameof(userPatientLinkToken)} not found");
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionVerifyResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId,
                        endUserSessionId);
                if (!demographicsResponse.HasSuccessResponse)
                {
                    LogExceptionError(nameof(_emisClient.DemographicsGet), demographicsResponse);
                    _logger.LogEmisErrorResponse(demographicsResponse);
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = demographicsResponse.Body?.ExtractNhsNumbers() ?? Enumerable.Empty<PatientNhsNumber>();

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = nhsNumbers
                };

                _logger.LogDebug("Verify successfully completed");
                return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to verify Emis Im1ConnectionToken, HttpRequestException has been thrown.");
                return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            try
            {
                _logger.LogEnter();
                
                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessResponse)
                {
                    LogExceptionError(nameof(_emisClient.SessionsEndUserSessionPost), endUserSessionResponse);
                    _logger.LogEmisErrorResponse(endUserSessionResponse);
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;

                _logger.LogDebug("Checking cache for IM1 connection token");
                var key = _im1CacheKeyGenerator.GenerateCacheKey(
                    request.AccountId, request.OdsCode, request.LinkageKey);
                var cachedConnectionToken =
                    await _im1CacheService.GetIm1ConnectionToken<EmisConnectionToken>(key);

                EmisConnectionToken connectionToken;
            
                if (cachedConnectionToken.HasValue)
                {
                    connectionToken = cachedConnectionToken.ValueOrFailure();
                    _logger.LogDebug("IM1 connection token found in cache.");
                }
                else
                {                    
                    var meApplicationsPostRequest = new MeApplicationsPostRequest
                    {
                        DateOfBirth = request.DateOfBirth,
                        Surname = request.Surname,
                        LinkageDetails = new LinkageDetails
                        {
                            AccountId = request.AccountId,
                            NationalPracticeCode = request.OdsCode,
                            LinkageKey = request.LinkageKey
                        }
                    };

                    var meApplicationsResponse =
                        await _emisClient.MeApplicationsPost(endUserSessionId, meApplicationsPostRequest);

                    var notFoundMessages = new[]
                    {
                        EmisApiErrorMessages.MeApplicationsPost_AccountIdNotFound,
                        EmisApiErrorMessages.MeApplicationsPost_LinkageKeyDoesNotMatch,
                        EmisApiErrorMessages.MeApplicationsPost_SurnameOrDateOfBirthAreIncorrect
                    };

                    if (meApplicationsResponse.StatusCode == HttpStatusCode.Conflict ||
                        meApplicationsResponse.HasExceptionWithMessage(
                            EmisApiErrorMessages.MeApplicationsPost_AlreadyLinked))
                    {
                        _logger.LogError(
                            $"Emis {nameof(_emisClient.MeApplicationsPost)} returned with statuscode {meApplicationsResponse.StatusCode}, account already exists");
                        _logger.LogEmisErrorResponse(meApplicationsResponse);
                        return new Im1ConnectionRegisterResult.AccountAlreadyExists();
                    }

                    if (meApplicationsResponse.HasExceptionWithAnyMessage(notFoundMessages))
                    {
                        LogExceptionError(nameof(_emisClient.MeApplicationsPost), meApplicationsResponse);
                        _logger.LogEmisErrorResponse(meApplicationsResponse);
                        return new Im1ConnectionRegisterResult.NotFound();
                    }

                    if (meApplicationsResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        LogExceptionError(nameof(_emisClient.MeApplicationsPost), meApplicationsResponse);
                        _logger.LogEmisErrorResponse(meApplicationsResponse);
                        return new Im1ConnectionRegisterResult.BadRequest();
                    }

                    if (!meApplicationsResponse.HasSuccessResponse)
                    {
                        _logger.LogError(
                            $"Emis {nameof(_emisClient.MeApplicationsPost)} returned with statuscode {meApplicationsResponse.StatusCode}, success");
                        _logger.LogEmisErrorResponse(meApplicationsResponse);
                        return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                    }

                    connectionToken = new EmisConnectionToken
                    {
                        AccessIdentityGuid = meApplicationsResponse.Body.AccessIdentityGuid,
                        Im1CacheKey = key
                    };

                    await CacheConnectionToken(connectionToken);
                }
                
                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = connectionToken.AccessIdentityGuid,
                    NationalPracticeCode = request.OdsCode
                };

                var sessionsResponse =
                    await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessResponse)
                {
                    LogExceptionError(nameof(_emisClient.SessionsPost), sessionsResponse);
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var userPatientLinkToken = sessionsResponse.Body?.ExtractUserPatientLinkToken();
                if (string.IsNullOrEmpty(userPatientLinkToken))
                {
                    _logger.LogError(
                        $"Emis could not extract {nameof(userPatientLinkToken)}");
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionRegisterResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId,
                        endUserSessionId);
                if (!demographicsResponse.HasSuccessResponse)
                {
                    LogExceptionError(nameof(_emisClient.DemographicsGet), demographicsResponse);
                    _logger.LogEmisErrorResponse(demographicsResponse);
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = demographicsResponse.Body?.ExtractNhsNumbers() ?? Enumerable.Empty<PatientNhsNumber>();

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken.SerializeJson(),
                    NhsNumbers = nhsNumbers
                };

                return new Im1ConnectionRegisterResult.SuccessfullyRegistered(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to register Emis Im1ConnectionToken, HttpRequestException has been thrown.");
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private void LogExceptionError<T>(string methodCall,
            EmisClient.EmisApiObjectResponse<T> response) =>
            _logger.LogError(response.GetExceptionLogMessage(methodCall));

        private async Task CacheConnectionToken(EmisConnectionToken connectionToken) =>
            await _im1CacheService.SaveIm1ConnectionToken(connectionToken.Im1CacheKey,
                connectionToken);
    }
}