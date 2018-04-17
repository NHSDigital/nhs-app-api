using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        public SessionCacheService(IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory =
                connectionMultiplexerFactory ?? throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
        }

        public async Task<string> CreateUserSession(UserSession userSession)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            var sessionExpirationTime = new TimeSpan(0,0,20,0); // redis session expiration time will be covered in another task. After that time session record will be removed from redis cache.
            RedisValue sessionObject = JsonConvert.SerializeObject(userSession);
            RedisKey sessionKey = Guid.NewGuid().ToString();

            await database.StringSetAsync(sessionKey, sessionObject, sessionExpirationTime);

            return sessionKey;
        }
    }
}
