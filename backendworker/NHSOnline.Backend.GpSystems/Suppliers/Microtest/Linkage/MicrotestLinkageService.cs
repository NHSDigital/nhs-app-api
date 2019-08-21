using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage
{
    public class MicrotestLinkageService : ILinkageService
    {
        private readonly IMicrotestClient _microtestClient;
        private readonly ILogger<MicrotestLinkageService> _logger;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;

        public MicrotestLinkageService(IMicrotestClient microtestClient, 
            ILogger<MicrotestLinkageService> logger,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService)
        {
            _microtestClient = microtestClient;
            _logger = logger;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            _logger.LogEnter();
            var demographicsResponse = await _microtestClient.DemographicsGet(
                    getLinkageRequest.OdsCode,
                    getLinkageRequest.NhsNumber);
            
            if (!demographicsResponse.HasSuccessResponse)
            {
                if (demographicsResponse.HasForbiddenResponse)
                {
                    _logger.LogError($"User does not have the necessary permissions within the GP system.");
                    _logger.LogExit();
                    return new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnknownError);
                }
                if (demographicsResponse.HasInternalServerError)
                {
                    _logger.LogError(
                        $"Internal server error when retrieving demographics as part of linkage. Status code: {(int) demographicsResponse.StatusCode}");
                    _logger.LogExit();
                    return new LinkageResult.InternalServerError();
                }
                _logger.LogError($"Unsuccessful request retrieving demographics as part of linkage. Status code: {(int)demographicsResponse.StatusCode}");
                return new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.LinkageKeysNotSupportedBySupplier);
            }
    
            var linkageKey = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var linkage = new LinkageResponse
            {
                AccountId = "microtest_" + accountId,
                LinkageKey = "microtest_" + linkageKey,
                OdsCode = getLinkageRequest.OdsCode
            };
    
            var key = _im1CacheKeyGenerator.GenerateCacheKey(
                linkage.AccountId, linkage.OdsCode, linkage.LinkageKey);

            var connectionToken = new MicrotestConnectionToken
            { 
                Im1CacheKey = key,
                NhsNumber = getLinkageRequest.NhsNumber,
            };

            await _im1CacheService.SaveIm1ConnectionToken(key, connectionToken);
            _logger.LogExit();
            return new LinkageResult.SuccessfullyRetrieved(linkage);
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            _logger.LogEnter();
            return await Task.FromResult(new LinkageResult.NotFound(Im1ConnectionErrorCodes.InternalCode.UnknownError));
        }
    }
}