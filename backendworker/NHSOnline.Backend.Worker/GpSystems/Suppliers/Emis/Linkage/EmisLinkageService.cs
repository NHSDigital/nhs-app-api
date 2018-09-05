using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.EmisClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageService : ILinkageService
    {
        private readonly ILogger _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisLinkageMapper _emisLinkageMapper;
        private readonly IEmisSessionService _emisSessionService;

        public EmisLinkageService(
            ILoggerFactory loggerFactory,
            IEmisClient emisClient,
            IEmisLinkageMapper emisLinkageMapper,
            IEmisSessionService emisSessionService)
        {
            _emisClient = emisClient;
            _emisLinkageMapper = emisLinkageMapper;
            _logger = loggerFactory.CreateLogger<EmisLinkageService>();
            _emisSessionService = emisSessionService;
        }

        public async Task<LinkageResult> GetLinkageKey(string nhsNumber, string odsCode, string identityToken)
        {
            try
            {
                var sessionPost = await _emisSessionService.SendSessionsEndUserSessionPost();

                var response = await GetLinkageKeyResponse(nhsNumber, odsCode, identityToken, sessionPost.EndUserSessionId);

                if (response.HasSuccessStatusCode || response.StatusCode == HttpStatusCode.Conflict)
                {
                    try
                    {
                        var linkage = _emisLinkageMapper.Map(response.Body);
                        linkage.OdsCode = odsCode;

                        if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            return new LinkageResult.SuccessfullyRetrievedAlreadyExists(linkage);
                        }

                        return new LinkageResult.SuccessfullyRetrieved(linkage);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");
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
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                var sessionPost = await _emisSessionService.SendSessionsEndUserSessionPost();

                var createLinkageKeyResponse = await CreateLinkageKey(createLinkageRequest, sessionPost.EndUserSessionId);

                if (!createLinkageKeyResponse.HasSuccessStatusCode)
                {
                    return GetErrorCreatingNhsUser(createLinkageKeyResponse);
                }

                var getLinkageKeyResponse = await GetLinkageKeyResponse(
                    createLinkageRequest.NhsNumber, createLinkageRequest.OdsCode, createLinkageRequest.IdentityToken, sessionPost.EndUserSessionId);

                if (getLinkageKeyResponse.HasSuccessStatusCode)
                {
                    try
                    {
                        var linkage = _emisLinkageMapper.Map(getLinkageKeyResponse.Body);
                        linkage.OdsCode = createLinkageRequest.OdsCode;
                        return new LinkageResult.SuccessfullyCreated(linkage);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");
                        return new LinkageResult.InternalServerError();
                    }
                }
                else if (getLinkageKeyResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    // This is straight after a successful creation, so the chance of
                    // getting a conflict retrieving the linkage key is incredibly unlikely.
                    // Returning a 409 from creation indicates that CID should call GET.
                    _logger.LogError($"Linkage create request unsuccessful - The patient already has an online account.");
                    
                    return new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount();
                }

                return GetErrorRetrievingNhsUser(getLinkageKeyResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request creating linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
            }
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

                    return new LinkageResult.PracticeNotLive();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientMarkedAsArchived))
                {
                    _logger.LogError($"Linkage get request unsuccessful - patient marked as archived. - Emis error code: {EmisApiErrorCode.PatientMarkedAsArchived}");

                    return new LinkageResult.PatientMarkedAsArchived();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientNonCompetentOrUnder16))
                {
                    _logger.LogError($"Linkage get request unsuccessful - patient non competent or under 16. - Emis error code: {EmisApiErrorCode.PatientNonCompetentOrUnder16}");

                    return new LinkageResult.PatientNonCompetentOrUnder16();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.AccountStatusInvalid))
                {
                    _logger.LogError($"Linkage get request unsuccessful - invalid account status. - Emis error code: {EmisApiErrorCode.AccountStatusInvalid}");

                    return new LinkageResult.AccountStatusInvalid();
                }

                _logger.LogError($"Linkage get request unsuccessful - Bad Request - Unknown error.");

                return new LinkageResult.BadRequestErrorRetrievingNhsUser();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.PatientNotRegisteredAtPractice))
                {
                    _logger.LogError($"Linkage get request unsuccessful - patient not registered at practice. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");

                    return new LinkageResult.PatientNotRegisteredAtPractice();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.NoRegisteredOnlineUserFound))
                {
                    _logger.LogError($"Linkage get request unsuccessful - no registered online user found. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");

                    return new LinkageResult.NoRegisteredOnlineUserFound();
                }
                
                _logger.LogError($"Linkage get request unsuccessful - Not Found - Unknown error.");


                return new LinkageResult.NotFoundErrorRetrievingNhsUser();
            }

            _logger.LogError("Linkage get request unsuccessful - Emis system is currently unavailable");

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
                  
                    return new LinkageResult.PracticeNotLive();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientMarkedAsArchived))
                {
                    _logger.LogError($"Linkage create request unsuccessful - patient marked as archived. - Emis error code: {EmisApiErrorCode.PatientMarkedAsArchived}");

                    return new LinkageResult.PatientMarkedAsArchived();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, EmisApiErrorCode.PatientNonCompetentOrUnder16))
                {
                    _logger.LogError($"Linkage create request unsuccessful - patient non competent or under 16. - Emis error code: {EmisApiErrorCode.PatientNonCompetentOrUnder16}");

                    return new LinkageResult.PatientNonCompetentOrUnder16();
                }
                
                _logger.LogError($"Linkage create request unsuccessful - Bad Request - Unknown error.");
                
                return new LinkageResult.BadRequestErrorCreatingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.PatientNotRegisteredAtPractice))
                {
                    _logger.LogError($"Linkage create request unsuccessful - patient not registered at practice. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");

                    return new LinkageResult.PatientNotRegisteredAtPractice();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, EmisApiErrorCode.NoRegisteredOnlineUserFound))
                {
                    _logger.LogError($"Linkage create request unsuccessful - no registered online user found. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");

                    return new LinkageResult.NoRegisteredOnlineUserFound();
                }

                _logger.LogError($"Linkage create request unsuccessful - no registered online user found. - Emis error code: {EmisApiErrorCode.PatientNotRegisteredAtPractice}");

                return new LinkageResult.NotFoundErrorCreatingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogError($"Linkage create request unsuccessful - The patient already has an online account.");

                return new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount();
            }

            _logger.LogError("Linkage create request unsuccessful - Emis system is currently unavailable");

            return new LinkageResult.SupplierSystemUnavailable();
        }
    }
}