using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal sealed class DualMongoIm1Cache : IMongoIm1Cache
    {
        private readonly ILogger _logger;
        private readonly MongoIm1CacheAccessor _primaryIm1CacheCache;
        private readonly MongoIm1CacheAccessor _secondaryIm1CacheCache;

        public DualMongoIm1Cache(
            ILogger<DualMongoIm1Cache> logger,
            IMongoClient mongoClient,
            IIm1CacheServiceConfig config)
        {
            _logger = logger;
            _primaryIm1CacheCache = new MongoIm1CacheAccessor(logger, mongoClient, config.DatabaseName, config.CollectionName);
            _secondaryIm1CacheCache = new MongoIm1CacheAccessor(logger, mongoClient, config.SecondaryDatabaseName, config.SecondaryCollectionName);
        }

        public async Task Save(string key, string token)
            => await _primaryIm1CacheCache.Save(key, token);

        public async Task<Option<string>> Get(string key)
        {
            var lookupResult = await _primaryIm1CacheCache.Get(key);
            if (lookupResult.IsEmpty)
            {
                lookupResult = await _secondaryIm1CacheCache.Get(key);
                await lookupResult
                    .IfSome(async encodedToken =>
                    {
                        _logger.LogInformation("Found Im1 Token in secondary cache Key={Key}", key);
                        await _primaryIm1CacheCache.Save(key, encodedToken);
                    })
                    .IfNone(() => Task.CompletedTask);
            }
            return lookupResult;
        }

        public async Task<bool> Delete(string key)
        {
            var results = await Task.WhenAll(_primaryIm1CacheCache.Delete(key), _secondaryIm1CacheCache.Delete(key));
            return results.Any(result => result);
        }
    }
}