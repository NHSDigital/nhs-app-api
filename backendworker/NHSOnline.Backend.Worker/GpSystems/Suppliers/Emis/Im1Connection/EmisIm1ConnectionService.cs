using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Im1Connection
{
    public class EmisIm1ConnectionService : IIm1ConnectionService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisIm1ConnectionService> _logger;
        private readonly IRegistrationGuidKeyGenerator _emisRegistrationGuidKeyGenerator;
        private readonly IRegistrationCacheService _registrationCacheService;

        public EmisIm1ConnectionService(IEmisClient emisClient, 
            ILogger<EmisIm1ConnectionService> logger,
            IRegistrationGuidKeyGenerator registrationGuidKeyGenerator,
            IRegistrationCacheService registrationCacheService)
        {
            _emisClient = emisClient;
            _logger = logger;
            _emisRegistrationGuidKeyGenerator = registrationGuidKeyGenerator;
            _registrationCacheService = registrationCacheService;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            try
            {
                _logger.LogEnter(nameof(Verify));

                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessStatusCode)
                {
                    LogExceptionError(nameof(_emisClient.SessionsEndUserSessionPost), endUserSessionResponse);
                    _logger.LogEmisErrorResponse(endUserSessionResponse);
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;
                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = connectionToken,
                    NationalPracticeCode = odsCode
                };

                var sessionsResponse = await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessStatusCode)
                {
                    LogExceptionError(nameof(_emisClient.SessionsPost), sessionsResponse);
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var userPatientLinkToken = sessionsResponse.Body.ExtractUserPatientLinkToken();
                if (string.IsNullOrEmpty(userPatientLinkToken))
                {
                    _logger.LogError("Emis userPatientLinkToken not found");
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionVerifyResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId,
                        endUserSessionId);
                if (!demographicsResponse.HasSuccessStatusCode)
                {
                    LogExceptionError(nameof(_emisClient.DemographicsGet), demographicsResponse);
                    _logger.LogEmisErrorResponse(demographicsResponse);
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = demographicsResponse.Body.ExtractNhsNumbers();

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
                _logger.LogExit(nameof(Verify));
            }
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            try
            {
                _logger.LogEnter(nameof(Register));
                
                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessStatusCode)
                {
                    LogExceptionError(nameof(_emisClient.SessionsEndUserSessionPost), endUserSessionResponse);
                    _logger.LogEmisErrorResponse(endUserSessionResponse);
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;
                string accessIdentityGuid = null;
                
                _logger.LogInformation("Checking Cache for AccessIdentityGuid");
                var key = _emisRegistrationGuidKeyGenerator.GenerateRegistrationKey(
                    request.AccountId, request.OdsCode, request.LinkageKey);
                var cachedGuid = await _registrationCacheService.GetRegistrationGuid(key);

                if (cachedGuid.HasValue)
                {
                    _logger.LogInformation("AccessIdentityGuid found in cache.");
                    accessIdentityGuid = cachedGuid.ValueOrFailure();
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
                            $"Emis MeApplicationsPost returned with statuscode {meApplicationsResponse.StatusCode}, account already exists");
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

                    if (!meApplicationsResponse.HasSuccessStatusCode)
                    {
                        _logger.LogError(
                            $"Emis MeApplicationsPost returned with statuscode {meApplicationsResponse.StatusCode}, success");
                        _logger.LogEmisErrorResponse(meApplicationsResponse);
                        return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                    }
                    accessIdentityGuid = meApplicationsResponse.Body.AccessIdentityGuid;
                }

                
                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = accessIdentityGuid,
                    NationalPracticeCode = request.OdsCode
                };

                var sessionsResponse =
                    await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessStatusCode)
                {
                    LogExceptionError(nameof(_emisClient.SessionsPost), sessionsResponse);
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var userPatientLinkToken = sessionsResponse.Body.ExtractUserPatientLinkToken();
                if (string.IsNullOrEmpty(userPatientLinkToken))
                {
                    _logger.LogError(
                        $"Emis could not extract UserPatientLinkToken");
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    return new Im1ConnectionRegisterResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId,
                        endUserSessionId);
                if (!demographicsResponse.HasSuccessStatusCode)
                {
                    LogExceptionError(nameof(_emisClient.DemographicsGet), demographicsResponse);
                    _logger.LogEmisErrorResponse(demographicsResponse);
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = demographicsResponse.Body.ExtractNhsNumbers();

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = accessIdentityGuid,
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
                _logger.LogExit(nameof(Register));
            }
        }

        private void LogExceptionError<T>(string methodCall, EmisClient.EmisApiObjectResponse<T> response)
        {
            _logger.LogError(response.GetExceptionLogMessage(methodCall));
        }

    }
}