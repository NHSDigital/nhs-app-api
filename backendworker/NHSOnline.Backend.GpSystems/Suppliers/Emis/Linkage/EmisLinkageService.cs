using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.GpSystems.Suppliers.Emis.EmisClient;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageService : ILinkageService
    {
        private readonly ILogger<EmisLinkageService> _logger;
        private readonly IEmisLinkageMapper _emisLinkageMapper;
        private readonly IEmisSessionService _emisSessionService;
        private readonly EmisLinkageServiceHelpers _linkageServiceHelpers;
        public EmisLinkageService(
            ILoggerFactory loggerFactory,
            IEmisLinkageMapper emisLinkageMapper,
            IEmisSessionService emisSessionService,
            EmisLinkageServiceHelpers linkageServiceHelpers)
        {
            _emisLinkageMapper = emisLinkageMapper;
            _logger = loggerFactory.CreateLogger<EmisLinkageService>();
            _emisSessionService = emisSessionService;
            _linkageServiceHelpers = linkageServiceHelpers;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            try
            {
                _logger.LogEnter();

                var sessionPost = await _emisSessionService.SendSessionsEndUserSessionPost();

                var response = await _linkageServiceHelpers.GetLinkageKeyResponse(getLinkageRequest.NhsNumber,
                    getLinkageRequest.OdsCode,
                    getLinkageRequest.IdentityToken,
                    sessionPost.EndUserSessionId);

                return IsSuccessfulResponse(response)
                    ? HandleGetLinkageKeyResponseSuccess(response, getLinkageRequest)
                    : EmisLinkageGetErrorMapper.Map(response, _logger);
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

                var createLinkageKeyResponse =
                    await _linkageServiceHelpers.CreateLinkageKey(createLinkageRequest, sessionPost.EndUserSessionId);

                if (!createLinkageKeyResponse.HasSuccessResponse)
                { 
                    return EmisLinkagePostErrorMapper.Map(createLinkageKeyResponse, _logger);
                }

                var getLinkageKeyResponse = await _linkageServiceHelpers.GetLinkageKeyResponse(
                    createLinkageRequest.NhsNumber,
                    createLinkageRequest.OdsCode,
                    createLinkageRequest.IdentityToken,
                    sessionPost.EndUserSessionId);

                if (IsSuccessfulResponse(getLinkageKeyResponse))
                {
                    return await HandleCreateLinkageKeyResponseSuccess(
                        getLinkageKeyResponse,
                        createLinkageRequest,
                        createLinkageKeyResponse);
                }
                return EmisLinkageGetErrorMapper.Map(getLinkageKeyResponse, _logger);
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

        private static bool IsSuccessfulResponse(EmisApiObjectResponse<AddVerificationResponse> response)
        {
            return response.HasSuccessResponse || response.StatusCode == HttpStatusCode.Conflict;
        }

        private LinkageResult HandleGetLinkageKeyResponseSuccess(
            EmisApiObjectResponse<AddVerificationResponse> response,
            GetLinkageRequest getLinkageRequest)
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
                return new LinkageResult.InternalServerError();
            }
        }

        private async Task<LinkageResult> HandleCreateLinkageKeyResponseSuccess(
            EmisApiObjectResponse<AddVerificationResponse> getLinkageKeyResponse,
            CreateLinkageRequest createLinkageRequest,
            EmisApiObjectResponse<AddNhsUserResponse> createLinkageKeyResponse)
        {
            try
            {
                var linkage = _emisLinkageMapper.Map(getLinkageKeyResponse.Body);
                linkage.OdsCode = createLinkageRequest.OdsCode;
                await _linkageServiceHelpers.StoreAccessGuidInCache(linkage, createLinkageKeyResponse.Body);
                return new LinkageResult.SuccessfullyCreated(linkage);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");
                return new LinkageResult.InternalServerError();
            }
        }
    }
}
