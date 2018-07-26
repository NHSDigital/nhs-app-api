using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Cipher;
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
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            var sessionExpirationTime = TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes);
            RedisValue sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);

            RedisKey sessionKey = Guid.NewGuid().ToString();

            sessionObject = _cipherService.Encrypt(sessionObject);

            await database.StringSetAsync(sessionKey, sessionObject, sessionExpirationTime);

            userSession.Key = sessionKey;
            return sessionKey;
        }

        public async Task<Option<UserSession>> GetUserSession(string sessionId)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            RedisKey redisKey = sessionId;
            var redisValue = await database.StringGetWithExpiryAsync(redisKey);

            if (redisValue.Value.IsNull)
            {
                _logger.LogDebug("No redis value Found");
                return Option.None<UserSession>();
            }

            await database.KeyExpireAsync(sessionId, TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes));

            var userSession = JsonConvert
                .DeserializeObject<UserSession>(_cipherService.Decrypt(redisValue.Value), _serializerSettings);

            userSession.Key = sessionId;

            return Option.Some(userSession);
        }

        public async Task<bool> DeleteUserSession(string sessionId)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            RedisKey redisKey = sessionId;
            
            return await database.KeyDeleteAsync(redisKey);
        }
    }
}