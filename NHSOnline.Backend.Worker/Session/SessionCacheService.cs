using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Support;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.Session
{
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(UserSession userSession);
        Task<Option<UserSession>> GetUserSession(string sessionId);
    }

    public class SessionCacheService : ISessionCacheService
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly ICipherService _cipherService;
        private readonly int _sessionExpiryMinutes;
        private readonly JsonSerializerSettings _serializerSettings;

        private const int DefaultSessionExpiryMinutes = 20;

        public SessionCacheService(IConnectionMultiplexerFactory connectionMultiplexerFactory,
            ICipherService cipherService, IConfiguration configuration)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory ??
                                            throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            _cipherService = cipherService;
            int.TryParse(configuration["SESSION_EXPIRY_MINUTES"], out _sessionExpiryMinutes);
            _sessionExpiryMinutes = _sessionExpiryMinutes == default(int)
                ? DefaultSessionExpiryMinutes
                : _sessionExpiryMinutes;
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public async Task<string> CreateUserSession(UserSession userSession)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            var sessionExpirationTime = TimeSpan.FromMinutes(_sessionExpiryMinutes);
            RedisValue sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);

            RedisKey sessionKey = Guid.NewGuid().ToString();

            sessionObject = _cipherService.Encrypt(sessionObject);

            await database.StringSetAsync(sessionKey, sessionObject, sessionExpirationTime);

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
                return Option.None<UserSession>();
            }

            await database.KeyExpireAsync(sessionId, TimeSpan.FromMinutes(_sessionExpiryMinutes));

            var userSession = JsonConvert
                .DeserializeObject<UserSession>(_cipherService.Decrypt(redisValue.Value), _serializerSettings);

            return Option.Some(userSession);
        }
    }
}