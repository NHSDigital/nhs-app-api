using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
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
            try
            {
                _logger.LogEnter();
                var demographicsResponse = await _microtestClient.DemographicsGet(
                        getLinkageRequest.OdsCode,
                        getLinkageRequest.NhsNumber);
                
                if (!demographicsResponse.HasSuccessResponse)
                {
                    _logger.LogError($"Unsuccessful request retrieving Microtest demographics as part of linkage. Status code: {(int)demographicsResponse.StatusCode}");
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
                return new LinkageResult.SuccessfullyRetrieved(linkage);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,
                    "Failed request retrieving Microtest demographics, Excception has been thrown.");
                return new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.LinkageKeysNotSupportedBySupplier);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            _logger.LogEnter();
            return await Task.FromResult(new LinkageResult.NotFound(Im1ConnectionErrorCodes.InternalCode.UnknownError));
        }
    }
}