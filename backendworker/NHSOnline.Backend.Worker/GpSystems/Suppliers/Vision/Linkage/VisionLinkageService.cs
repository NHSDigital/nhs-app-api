using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage
{
    public class VisionLinkageService : ILinkageService
    {
        private readonly ILogger<VisionLinkageService> _logger;
        private readonly IVisionClient _visionClient;
        private readonly IVisionLinkageMapper _visionLinkageMapper;
        private readonly IIm1CacheService _im1CacheService;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;

        public VisionLinkageService(
            ILoggerFactory loggerFactory,
            IVisionClient visionClient,
            IVisionLinkageMapper visionLinkageMapper,
            IIm1CacheService im1CacheService,
            IIm1CacheKeyGenerator im1CacheKeyGenerator)
        {
            _logger = loggerFactory.CreateLogger<VisionLinkageService>();
            _visionClient = visionClient;
            _visionLinkageMapper = visionLinkageMapper;
            _im1CacheService = im1CacheService;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            try
            {
                _logger.LogEnter();

                var request = new GetLinkageKey
                {
                    NhsNumber = getLinkageRequest.NhsNumber,
                    OdsCode = getLinkageRequest.OdsCode,
                };

                var response = await _visionClient.GetLinkageKey(request);

                if (response.HasSuccessResponse)
                {
                    try
                    {
                        var mapped = _visionLinkageMapper.Map(response.Body);
                        
                        return new LinkageResult.SuccessfullyRetrieved(mapped);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");
                        return new LinkageResult.InternalServerError();
                    }
                }

                return GetErrorRetrievingLinkageKey(response);
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

                var request = new CreateLinkageKey
                {
                    OdsCode = createLinkageRequest.OdsCode,
                    LinkageKeyPostRequest = new LinkageKeyPostRequest
                    {
                        NhsNumber = createLinkageRequest.NhsNumber,
                        LastName = createLinkageRequest.Surname,
                        DateOfBirth = createLinkageRequest.DateOfBirth.FormatToYYYYMMDD(),
                    },
                };
                
                var linkageResponse = await _visionClient.CreateLinkageKey(request);
                
                if (linkageResponse.HasSuccessResponse)
                {
                    var mapped = _visionLinkageMapper.Map(linkageResponse.Body);
                    await StoreAccessGuidInCache(mapped, linkageResponse.Body.ApiKey);
                    return new LinkageResult.SuccessfullyCreated(mapped);
                }
                else if (linkageResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    _logger.LogError(
                        $"Linkage create request unsuccessful - The patient already has an online account. {JsonConvert.SerializeObject(linkageResponse)}");
                    return new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount();
                }

                return GetErrorCreatingLinkageKey(linkageResponse);
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

        private async Task StoreAccessGuidInCache(LinkageResponse linkage, string apiKey)
        {
            var key = _im1CacheKeyGenerator.GenerateCacheKey(linkage.AccountId, linkage.OdsCode, linkage.LinkageKey);

            var connectionToken = new VisionConnectionToken
            {
                ApiKey = apiKey,
                RosuAccountId = linkage.AccountId,
            };

            await _im1CacheService.SaveIm1ConnectionToken(key, connectionToken);
        }

        private LinkageResult GetErrorRetrievingLinkageKey(VisionLinkageClient.VisionApiObjectResponse<LinkageKeyGetResponse> response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, VisionApiErrorCodes.InvalidNhsNumber))
                {
                    LogLinkageGetError(response, "Bad Request - Invalid NHS Number");
                    return new LinkageResult.BadRequestErrorRetrievingNhsUser();
                }

                LogLinkageGetError(response, "Bad Request - Unknown error");
                return new LinkageResult.BadRequestErrorRetrievingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, VisionApiErrorCodes.PatientRecordNotFound))
                {
                    LogLinkageGetError(response, "Not found - Patient record not found");
                    return new LinkageResult.NotFoundErrorRetrievingNhsUser();
                }

                LogLinkageGetError(response, "Not found - Unknown error");
                return new LinkageResult.NotFoundErrorRetrievingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.Forbidden, VisionApiErrorCodes.LinkageKeyRevoked))
                {
                    LogLinkageGetError(response, "Forbidden - Linkage key revoked");
                    return new LinkageResult.LinkageKeyRevoked();
                }

                LogLinkageGetError(response, "Forbidden - Unknown error");
                return new LinkageResult.ForbiddenErrorRetrievingNhsUser();
            }

            LogLinkageGetError(response, "Unknown error");
            return new LinkageResult.SupplierSystemUnavailable();
        }

        private void LogLinkageGetError(VisionLinkageClient.VisionApiObjectResponse<LinkageKeyGetResponse> response, string message)
        {
            _logger.LogError($"Linkage get request unsuccessful - { message } - Status code: { response.StatusCode } - Vision error: { response.ErrorForLogging }");
        }

        private LinkageResult GetErrorCreatingLinkageKey(
            VisionLinkageClient.VisionApiObjectResponse<LinkageKeyPostResponse> response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest, VisionApiErrorCodes.InvalidNhsNumber))
                {
                    LogLinkageCreateError(response, "Bad Request - Invalid NHS Number");
                    return new LinkageResult.BadRequestErrorCreatingNhsUser();
                }

                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest,
                    VisionApiErrorCodes.LinkageKeyRevoked))
                {
                    LogLinkageCreateError(response, "Bad Request - Linkage key revoked");
                    return new LinkageResult.AccountStatusInvalid();
                }

                LogLinkageCreateError(response, "Bad Request - Unknown error");
                return new LinkageResult.BadRequestErrorCreatingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.NotFound, VisionApiErrorCodes.InvalidNhsNumber))
                {
                    LogLinkageCreateError(response, "Not found - Patient record not found");
                    return new LinkageResult.NotFoundErrorCreatingNhsUser();
                }

                LogLinkageCreateError(response, "Not Found - Unknown error");
                return new LinkageResult.NotFoundErrorCreatingNhsUser();
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                if (response.HasStatusCodeAndErrorCode(HttpStatusCode.Conflict,
                    VisionApiErrorCodes.LinkageKeyAlreadyExists))
                {
                    LogLinkageCreateError(response, "Conflict - The patient already has an online account");
                    return new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount();
                }
                
                LogLinkageCreateError(response, "Conflict - Unknown error");
                return new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount();
            }

            LogLinkageCreateError(response, "Unknown error");
            return new LinkageResult.SupplierSystemUnavailable();
        }

        private void LogLinkageCreateError(VisionLinkageClient.VisionApiObjectResponse<LinkageKeyPostResponse> response,
            string message)
        {
            _logger.LogError(
                $"Linkage create request unsuccessful - {message} - Status code: { response.StatusCode } - Vision error: {response.ErrorForLogging}");
        }
    }
}