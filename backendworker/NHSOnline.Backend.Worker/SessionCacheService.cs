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

        Task UpdateUserSession(UserSession userSession);
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
                sessionObject = _cipherService.Encrypt(sessionObject);
                RedisKey sessionKey = Guid.NewGuid().ToString();


                using (_logger.WithTimer("Add session to Redis"))
                {
                    await database.StringSetAsync(sessionKey, sessionObject, sessionExpirationTime);
                }

                userSession.Key = sessionKey;
                return sessionKey;
            }
            finally
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

                RedisValueWithExpiry redisValue;
                using (_logger.WithTimer("Get session from Redis"))
                {
                    redisValue = await database.StringGetWithExpiryAsync(redisKey);
                }

                if (redisValue.Value.IsNull)
                {
                    _logger.LogDebug("No redis value Found");
                    return Option.None<UserSession>();
                }

                using (_logger.WithTimer("Update session expiry in Redis"))
                {
                    await database.KeyExpireAsync(sessionId, TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes));
                }


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


                using (_logger.WithTimer("Delete session in Redis"))
                {
                    return await database.KeyDeleteAsync(redisKey);
                }
            }
            finally
            {
                _logger.LogExit(nameof(DeleteUserSession));
            }
        }

        public async Task UpdateUserSession(UserSession userSession)
        {
            try
            {
                _logger.LogEnter(nameof(UpdateUserSession));
                var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
                var database = multiplexer.GetDatabase();
                var sessionExpirationTime = TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes);

                RedisValue sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);
                sessionObject = _cipherService.Encrypt(sessionObject);

                using (_logger.WithTimer("Update Redis session"))
                {
                    await database.StringSetAsync(userSession.Key, sessionObject, sessionExpirationTime);
                }
            }
            finally
            {
                _logger.LogExit(nameof(UpdateUserSession));
            }
        }

    }
}