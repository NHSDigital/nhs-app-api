using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.DataProtection;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.Session
{
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(UserSession userSession);
    }

    public class SessionCacheService : ISessionCacheService
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly ICipherService _cipherService;

        public SessionCacheService(IConnectionMultiplexerFactory connectionMultiplexerFactory, ICipherService cipherService)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory ?? throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            _cipherService = cipherService;
        }

        public async Task<string> CreateUserSession(UserSession userSession)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            var sessionExpirationTime = TimeSpan.FromMinutes(20); // redis session expiration time will be covered in another task. After that time session record will be removed from redis cache.
            RedisValue sessionObject = JsonConvert.SerializeObject(userSession);
            RedisKey sessionKey = Guid.NewGuid().ToString();

            sessionObject = _cipherService.Encrypt(sessionObject);

            await database.StringSetAsync(sessionKey, sessionObject, sessionExpirationTime);

            return sessionKey;
        }
    }
}
