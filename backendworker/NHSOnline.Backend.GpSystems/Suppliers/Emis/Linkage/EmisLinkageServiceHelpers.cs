using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using static NHSOnline.Backend.GpSystems.Suppliers.Emis.EmisClient;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageServiceHelpers
    {
        private readonly IEmisClient _emisClient;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;

        public EmisLinkageServiceHelpers(
            IEmisClient emisClient,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService)
        {
            _emisClient = emisClient;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
        }

        internal async Task StoreAccessGuidInCache(LinkageResponse linkage, AddNhsUserResponse addNhsUserResponse)
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

        internal async Task<EmisApiObjectResponse<AddVerificationResponse>> GetLinkageKeyResponse(string nhsNumber,
            string odsCode, string identityToken, string endUserSessionId)
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

        internal async Task<EmisApiObjectResponse<AddNhsUserResponse>> CreateLinkageKey(
            CreateLinkageRequest createLinkageRequest, string endUserSessionId)
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
    }
}
