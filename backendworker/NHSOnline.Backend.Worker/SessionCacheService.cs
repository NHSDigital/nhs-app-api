using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Cipher;
using NHSOnline.Backend.Worker.Support.Logging;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(UserSession userSession);
        Task<Option<UserSession>> GetUserSession(string sessionId);
        Task<bool> DeleteUserSession(string sessionId);        
    }

    public class SessionCacheService : ISessionCacheService
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly ICipherService _cipherService;        
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<SessionCacheService> _logger;

        public SessionCacheService(
            IConnectionMultiplexerFactory connectionMultiplexerFactory,
            ICipherService cipherService, 
            IOptions<ConfigurationSettings> settings,
            ILogger<SessionCacheService> logger)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory ??
                                            throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            _settings = settings.Value;
            _cipherService = cipherService;

            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            _logger = logger;
        }

        public async Task<string> CreateUserSession(UserSession userSession)
        {
            try
            {
                _logger.LogEnter(nameof(CreateUserSession));
                var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
                var database = multiplexer.GetDatabase();
                var sessionExpirationTime = TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes);
                RedisValue sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);

                RedisKey sessionKey = Guid.NewGuid().ToString();

                sessionObject = _cipherService.Encrypt(sessionObject);

                _logger.LogDebug("Adding session to Redis");
                await database.StringSetAsync(sessionKey, sessionObject, sessionExpirationTime);
                _logger.LogDebug("Added session to Redis");

                userSession.Key = sessionKey;
                return sessionKey;
            }finally
            {
                _logger.LogExit(nameof(CreateUserSession));
            }
        }

        public async Task<Option<UserSession>> GetUserSession(string sessionId)
        {
            try
            {
                _logger.LogEnter(nameof(GetUserSession));

                var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
                var database = multiplexer.GetDatabase();
                RedisKey redisKey = sessionId;

                _logger.LogDebug("Retrieving session from Redis");
                var redisValue = await database.StringGetWithExpiryAsync(redisKey);
                _logger.LogDebug("Retrieved session from Redis");

                if (redisValue.Value.IsNull)
                {
                    _logger.LogDebug("No redis value Found");
                    return Option.None<UserSession>();
                }

                _logger.LogDebug("Updating expiry for session in Redis");
                await database.KeyExpireAsync(sessionId, TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes));
                _logger.LogDebug("Updated expiry for session in Redis");

                var userSession = JsonConvert
                    .DeserializeObject<UserSession>(_cipherService.Decrypt(redisValue.Value), _serializerSettings);

                userSession.Key = sessionId;

                return Option.Some(userSession);
            }
            finally
            {
                _logger.LogExit(nameof(GetUserSession));
            }
        }

        public async Task<bool> DeleteUserSession(string sessionId)
        {
            try
            {
                _logger.LogEnter(nameof(DeleteUserSession));

                var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
                var database = multiplexer.GetDatabase();
                RedisKey redisKey = sessionId;

                _logger.LogDebug("Deleting session from Redis");
                var result = await database.KeyDeleteAsync(redisKey);
                _logger.LogDebug("Deleted session from Redis");

                return result;
            }
            finally
            {
                _logger.LogExit(nameof(DeleteUserSession));
            }
        }
    }
}