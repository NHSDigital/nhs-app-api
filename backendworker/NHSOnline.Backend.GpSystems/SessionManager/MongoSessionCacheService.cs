using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal sealed class MongoSessionCacheService : ISessionCacheService
    {
        private readonly ILogger<MongoSessionCacheService> _logger;
        private readonly UserSessionEncryptionService _encryptionService;
        private readonly MongoSessionCacheAccessor _cacheAccessor;

        public MongoSessionCacheService(
            ILogger<MongoSessionCacheService> logger,
            IMongoClient mongoClient,
            IMongoSessionCacheServiceConfig config,
            UserSessionEncryptionService encryptionService)
        {
            _logger = logger;
            _encryptionService = encryptionService;
            _cacheAccessor = new MongoSessionCacheAccessor(logger, mongoClient, config.DatabaseName, config.CollectionName);
        }

        public async Task<string> CreateUserSession(UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var encodedUserSession = _encryptionService.Encode(userSession);
                var sessionKey = await _cacheAccessor.Create(encodedUserSession);

                userSession.Key = sessionKey;
                return sessionKey;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Option<UserSession>> GetUserSession(string sessionId)
        {
            try
            {
                _logger.LogEnter();

                var encodedUserSession = await _cacheAccessor.Get(sessionId);
                return encodedUserSession.Select(RecreateUserSession);
            }
            finally
            {
                _logger.LogExit();
            }

            UserSession RecreateUserSession(string encodedUserSession)
            {
                var userSession = _encryptionService.Decode(encodedUserSession);
                userSession.Key = sessionId;
                return userSession;
            }
        }

        public async Task<bool> DeleteUserSession(string sessionId)
        {
            try
            {
                _logger.LogEnter();

                return await _cacheAccessor.Delete(sessionId);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task UpdateUserSession(UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var encodedUserSession = _encryptionService.Encode(userSession);
                await _cacheAccessor.Update(userSession.Key, encodedUserSession);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}