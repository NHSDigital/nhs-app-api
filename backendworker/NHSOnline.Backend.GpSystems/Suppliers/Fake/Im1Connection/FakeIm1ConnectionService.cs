using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Models.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection
{
    public class FakeIm1ConnectionService : FakeServiceBase, IIm1ConnectionService
    {
        private readonly ILogger<FakeIm1ConnectionService> _logger;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;

        public FakeIm1ConnectionService(
            ILogger<FakeIm1ConnectionService> logger,
            IFakeUserRepository fakeUserRepository,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            _logger.LogEnter();

            try
            {
                var token = connectionToken.DeserializeJson<FakeConnectionToken>();

                var fakeUser = await FindUser(token.NhsNumber);
                return await fakeUser.Im1ConnectionAreaBehaviour.Verify(connectionToken, odsCode);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            _logger.LogEnter();

            try
            {
                var key = _im1CacheKeyGenerator.GenerateCacheKey(
                    request.AccountId, request.OdsCode, request.LinkageKey);

                var cachedConnectionToken =
                    await _im1CacheService.GetIm1ConnectionToken<FakeConnectionToken>(key);

                if (cachedConnectionToken.HasValue)
                {
                    var connectionToken = cachedConnectionToken.ValueOrFailure();
                    _logger.LogDebug("IM1 connection token found in cache.");

                    var fakeUser = await FindUser(connectionToken.NhsNumber);

                    return await fakeUser.Im1ConnectionAreaBehaviour.Register(request, connectionToken);
                }

                _logger.LogDebug("IM1 connection token not found in cache.");
                return await Task.FromResult<Im1ConnectionRegisterResult>(
                    new Im1ConnectionRegisterResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode
                    .LinkageKeysNotSupportedBySupplier));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}