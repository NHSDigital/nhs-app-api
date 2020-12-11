using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal sealed class Im1CacheService : IIm1CacheService
    {
        internal const string Im1ConnectionTokenCacheKeyPropertyName = "Im1CacheKey";

        private readonly ILogger _logger;

        private readonly Im1TokenEncryptionService _encryptionService;
        private readonly MongoIm1CacheAccessor _cacheAccessor;

        public Im1CacheService(
            ILogger<Im1CacheService> logger,
            Im1TokenEncryptionService encryptionService,
            IMongoClient mongoClient,
            IIm1CacheServiceConfig config)
        {
            _logger = logger;
            _encryptionService = encryptionService;

            _cacheAccessor = new MongoIm1CacheAccessor(logger, mongoClient, config.MongoDatabaseName, config.MongoDatabaseIm1CollectionName);
        }

        public string CacheKeyPropertyName => Im1ConnectionTokenCacheKeyPropertyName;

        public async Task SaveIm1ConnectionToken<T>(string key, T value)
        {
            try
            {
                _logger.LogEnter();

                var connectionToken = _encryptionService.Encode(value);
                await _cacheAccessor.Save(key, connectionToken);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Option<T>> GetIm1ConnectionToken<T>(string key)
        {
            try
            {
                _logger.LogEnter();

                var cachedValue = await _cacheAccessor.Get(key);

                return cachedValue.Select(_encryptionService.Decode<T>);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<bool> DeleteIm1ConnectionToken(string key)
        {
            try
            {
                _logger.LogEnter();

                return await _cacheAccessor.Delete(key);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}