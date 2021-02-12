using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal sealed class MongoSessionCacheService : ISessionCacheService
    {
        private readonly ILogger<MongoSessionCacheService> _logger;
        private readonly UserSessionEncryptionService _encryptionService;
        private readonly IMongoSessionCache _mongoSessionCache;

        public MongoSessionCacheService(
            ILogger<MongoSessionCacheService> logger,
            UserSessionEncryptionService encryptionService,
            IMongoSessionCache mongoSessionCache)
        {
            _logger = logger;
            _encryptionService = encryptionService;
            _mongoSessionCache = mongoSessionCache;
        }

        public async Task<string> CreateUserSession(UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var sessionId = Guid.NewGuid().ToString();
                var encodedUserSession = _encryptionService.Encode(userSession);

                await _mongoSessionCache.Create(sessionId, encodedUserSession);

                userSession.Key = sessionId;
                return sessionId;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Option<UserSession>> GetAndUpdateUserSession(string sessionId)
        {
            try
            {
                _logger.LogEnter();

                var encodedUserSession = await _mongoSessionCache.GetAndUpdate(sessionId);
                return encodedUserSession.Select(x =>  RecreateUserSession(x, sessionId));
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

                var encodedUserSession = await _mongoSessionCache.Get(sessionId);
                return encodedUserSession.Select(x =>  RecreateUserSession(x, sessionId));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<bool> DeleteUserSession(string sessionId)
        {
            try
            {
                _logger.LogEnter();

                return await _mongoSessionCache.Delete(sessionId);
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
                await _mongoSessionCache.Update(userSession.Key, encodedUserSession);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private UserSession RecreateUserSession(string encodedUserSession, string sessionId)
        {
            var userSession = _encryptionService.Decode(encodedUserSession);
            userSession.Key = sessionId;
            return userSession;
        }
    }
}
