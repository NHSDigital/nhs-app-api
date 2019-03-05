using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.GpSystems.Suppliers.Emis.EmisClient;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageService : ILinkageService
    {
        private readonly ILogger<EmisLinkageService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisLinkageMapper _emisLinkageMapper;
        private readonly IEmisSessionService _emisSessionService;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;

        public EmisLinkageService(
            ILoggerFactory loggerFactory,
            IEmisClient emisClient,
            IEmisLinkageMapper emisLinkageMapper,
            IEmisSessionService emisSessionService,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService)
        {
            _emisClient = emisClient;
            _emisLinkageMapper = emisLinkageMapper;
            _logger = loggerFactory.CreateLogger<EmisLinkageService>();
            _emisSessionService = emisSessionService;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            try
            {
                _logger.LogEnter();
           
                var sessionPost = await _emisSessionService.SendSessionsEndUserSessionPost();

                var response = await GetLinkageKeyResponse(getLinkageRequest.NhsNumber, getLinkageRequest.OdsCode, 
                    getLinkageRequest.IdentityToken, sessionPost.EndUserSessionId);

                if (response.HasSuccessResponse || response.StatusCode == HttpStatusCode.Conflict)
                {
                    try
                    {
                        var linkage = _emisLinkageMapper.Map(response.Body);
                        linkage.OdsCode = getLinkageRequest.OdsCode;

                        if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            return new LinkageResult.SuccessfullyRetrievedAlreadyExists(linkage);
                        }

                        return new LinkageResult.SuccessfullyRetrieved(linkage);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");
                        _logger.LogEmisErrorResponse(response);
                        return new LinkageResult.InternalServerError();
                    }
                }

                return GetErrorRetrievingNhsUser(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                _logger.LogEnter();

                var sessionPost = await _emisSessionService.SendSessionsEndUserSessionPost();

                var createLinkageKeyResponse = await CreateLinkageKey(createLinkageRequest, sessionPost.EndUserSessionId);

                if (!createLinkageKeyResponse.HasSuccessResponse)
                {
                    return GetErrorCreatingNhsUser(createLinkageKeyResponse);
                }

                var getLinkageKeyResponse = await GetLinkageKeyResponse(
                    createLinkageRequest.NhsNumber, createLinkageRequest.OdsCode, createLinkageRequest.IdentityToken, sessionPost.EndUserSessionId);

                if (getLinkageKeyResponse.HasSuccessResponse)
                {
                    try
                    {
                        var linkage = _emisLinkageMapper.Map(getLinkageKeyResponse.Body);
                        linkage.OdsCode = createLinkageRequest.OdsCode;
                        await StoreAccessGuidInCache(linkage, createLinkageKeyResponse.Body);
                        return new LinkageResult.SuccessfullyCreated(linkage);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");
                        _logger.LogEmisErrorResponse(getLinkageKeyResponse);
                        return new LinkageResult.InternalServerError();
                    }
                }
                else if (getLinkageKeyResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    // This is straight after a successful creation, so the chance of
                    // getting a conflict retrieving the linkage key is incredibly unlikely.
                    // Returning a 409 from creation indicates that CID should call GET.
                    _logger.LogError($"Linkage create request unsuccessful - The patient already has an online account.");
                    _logger.LogEmisErrorResponse(getLinkageKeyResponse);
                    return new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount();
                }

                return GetErrorRetrievingNhsUser(getLinkageKeyResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request creating linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task StoreAccessGuidInCache(LinkageResponse linkage, AddNhsUserResponse addNhsUserResponse)
        {
            var key = _im1CacheKeyGenerator.GenerateCacheKey(
                    linkage.AccountId, linkage.OdsCode, linkage.LinkageKey);

            var connectionToken = new EmisConnectionToken
            {
                AccessIdentityGuid = addNhsUserResponse.AccessIdentityGuid.ToString(),
                Im1CacheKey = key,
            };

            await _im1CacheService.SaveIm1ConnectionToken(key, connectionToken);
        }

        private async Task<EmisApiObjectResponse<AddVerificationResponse>> GetLinkageKeyResponse(string nhsNumber, string odsCode, string identityToken, string endUserSessionId)
        {
            var addVerificationRequest = new AddVerificationRequest
            {
                NhsNumber = nhsNumber,
                NationalPracticeCode = odsCode,
                Token = identityToken,
            };

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var headerParams = new EmisHeaderParameters(emisUserSession);

            var linkageResponse = await _emisClient.VerificationPost(headerParams, addVerificationRequest);

            return linkageResponse;
        }

        private LinkageResult GetErrorRetrievingNhsUser(EmisApiObjectResponse<AddVerificationResponse> response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PracticeNotLive))
                {
                    _logger.LogError($"Linkage get request unsuccessful - practice not live. - Emis error code: {EmisApiErrorCode.PracticeNotLive}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PracticeNotLive();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientMarkedAsArchived))
                {
                    _logger.LogError($"Linkage get request unsuccessful - patient marked as archived. - Emis error code: {EmisApiErrorCode.PatientMarkedAsArchived}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PatientMarkedAsArchived();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientNonCompetentOrUnderMinimumAge))
                {
                    _logger.LogError($"Linkage get request unsuccessful - patient non competent or under minimum age. - Emis error code: {EmisApiErrorCode.PatientNonCompetentOrUnderMinimumAge}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PatientNonCompetentOrUnderMinimumAge();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.AccountStatusInvalid))
                {
                    _logger.LogError($"Linkage get request unsuccessful - invalid account status. - Emis error code: {EmisApiErrorCode.AccountStatusInvalid}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.AccountStatusInvalid();
                }

                _logger.LogError($"Linkage get request unsuccessful - Bad Request - Unknown error.");
                _logger.LogEmisErrorResponse(response);
                return new LinkageResult.BadRequestErrorRetrievingNhsUser();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.PatientNotRegisteredAtPractice))
                {
                    _logger.LogError($"Linkage get request unsuccessful - patient not registered at practice. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PatientNotRegisteredAtPractice();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.NoRegisteredOnlineUserFound))
                {
                    _logger.LogError($"Linkage get request unsuccessful - no registered online user found. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.NoRegisteredOnlineUserFound();
                }
                
                _logger.LogError($"Linkage get request unsuccessful - Not Found - Unknown error.");

                _logger.LogEmisErrorResponse(response);
                return new LinkageResult.NotFoundErrorRetrievingNhsUser();
            }

            _logger.LogError("Linkage get request unsuccessful - Emis system is currently unavailable");
            _logger.LogEmisErrorResponse(response);
            return new LinkageResult.SupplierSystemUnavailable();
        }

        private async Task<EmisApiObjectResponse<AddNhsUserResponse>> CreateLinkageKey(CreateLinkageRequest createLinkageRequest, string endUserSessionId)
        {
            var request = new AddNhsUserRequest
            {
                NhsNumber = createLinkageRequest.NhsNumber,
                NationalPracticeCode = createLinkageRequest.OdsCode,
                EmailAddress = createLinkageRequest.EmailAddress,
            };

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var headerParams = new EmisHeaderParameters(emisUserSession);

            var createNhsUserResponse = await _emisClient.NhsUserPost(headerParams, request);
            
            return createNhsUserResponse;
        }

        private LinkageResult GetErrorCreatingNhsUser(EmisApiObjectResponse<AddNhsUserResponse> response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PracticeNotLive))
                {
                    _logger.LogError($"Linkage create request unsuccessful - practice not live. - Emis error code: {EmisApiErrorCode.PracticeNotLive}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PracticeNotLive();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientMarkedAsArchived))
                {
                    _logger.LogError($"Linkage create request unsuccessful - patient marked as archived. - Emis error code: {EmisApiErrorCode.PatientMarkedAsArchived}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PatientMarkedAsArchived();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientNonCompetentOrUnderMinimumAge))
                {
                    _logger.LogError($"Linkage create request unsuccessful - patient non competent or under minimum age. - Emis error code: {EmisApiErrorCode.PatientNonCompetentOrUnderMinimumAge}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PatientNonCompetentOrUnderMinimumAge();
                }
                
                _logger.LogError($"Linkage create request unsuccessful - Bad Request - Unknown error.");
                _logger.LogEmisErrorResponse(response);
                return new LinkageResult.BadRequestErrorCreatingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.PatientNotRegisteredAtPractice))
                {
                    _logger.LogError($"Linkage create request unsuccessful - patient not registered at practice. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.PatientNotRegisteredAtPractice();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.NoRegisteredOnlineUserFound))
                {
                    _logger.LogError($"Linkage create request unsuccessful - no registered online user found. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");
                    _logger.LogEmisErrorResponse(response);
                    return new LinkageResult.NoRegisteredOnlineUserFound();
                }

                _logger.LogError($"Linkage create request unsuccessful - no registered online user found. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");
                _logger.LogEmisErrorResponse(response);
                return new LinkageResult.NotFoundErrorCreatingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogError($"Linkage create request unsuccessful - The patient already has an online account.");
                _logger.LogEmisErrorResponse(response);
                return new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount();
            }

            _logger.LogError("Linkage create request unsuccessful - Emis system is currently unavailable");
            _logger.LogEmisErrorResponse(response);
            return new LinkageResult.SupplierSystemUnavailable();
        }
    }
}