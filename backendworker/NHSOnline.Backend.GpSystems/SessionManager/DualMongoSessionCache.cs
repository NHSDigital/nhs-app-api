using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal sealed class DualMongoSessionCache : IMongoSessionCache
    {
        private readonly ILogger _logger;
        private readonly MongoSessionCacheAccessor _primarySessionCache;
        private readonly MongoSessionCacheAccessor _secondarySessionCache;

        public DualMongoSessionCache(
            ILogger<DualMongoSessionCache> logger,
            IMongoClient mongoClient,
            IMongoSessionCacheServiceConfig config)
        {
            _logger = logger;
            _primarySessionCache = new MongoSessionCacheAccessor(logger, mongoClient, config.DatabaseName, config.CollectionName);
            _secondarySessionCache = new MongoSessionCacheAccessor(logger, mongoClient, config.SecondaryDatabaseName, config.SecondaryCollectionName);
        }

        public async Task Create(string sessionId, string encodedUserSession)
            => await _primarySessionCache.Create(sessionId, encodedUserSession);

        public async Task<Option<string>> Get(string sessionId)
        {
            var lookupResult = await _primarySessionCache.Get(sessionId);

            if (lookupResult.IsEmpty)
            {
                lookupResult = await _secondarySessionCache.Get(sessionId);
                await lookupResult
                    .IfSome(async encodedUserSession =>
                    {
                        _logger.LogInformation("Found session in secondary cache SessionId={SessionId}", sessionId);
                        await _primarySessionCache.Create(sessionId, encodedUserSession);
                    })
                    .IfNone(() => Task.CompletedTask);
            }

            return lookupResult;
        }

        public async Task<bool> Delete(string sessionId)
        {
            var results = await Task.WhenAll(_primarySessionCache.Delete(sessionId), _secondarySessionCache.Delete(sessionId));
            return results.Any(result => result);
        }

        public async Task Update(string key, string encodedUserSession)
            => await _primarySessionCache.Update(key, encodedUserSession);
    }
}