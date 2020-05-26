using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Models.Im1Connection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage
{
    public class DefaultLinkageBehaviour : ILinkageBehaviour
    {
        private readonly IIm1CacheService _im1CacheService;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;

        public DefaultLinkageBehaviour(
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService)
        {
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            var accountId = Guid.NewGuid();
            var linkageKey = Guid.NewGuid();

            var linkage = new LinkageResponse
            {
                AccountId = "fake_" + accountId,
                LinkageKey = "fake_" + linkageKey,
                OdsCode = getLinkageRequest.OdsCode
            };

            var key = _im1CacheKeyGenerator.GenerateCacheKey(
                linkage.AccountId, linkage.OdsCode, linkage.LinkageKey);

            var connectionToken = new FakeConnectionToken
            {
                Im1CacheKey = key,
                NhsNumber = getLinkageRequest.NhsNumber,
            };

            await _im1CacheService.SaveIm1ConnectionToken(key, connectionToken);

            return new LinkageResult.SuccessfullyRetrieved(linkage);
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            return await Task.FromResult(new LinkageResult.NotFound(Im1ConnectionErrorCodes.InternalCode.UnknownError));
        }
    }
}