using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Hasher;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public interface IRegistrationCacheService
    {
        Task<string> CreateRegistrationToken<T>(string key, T value);
        Task<Option<T>> GetRegistrationToken<T>(string key);
        Task<bool> DeleteRegistrationToken(string key);
    }
    
    public class RegistrationCacheService : IRegistrationCacheService
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly IHashingService _hashingService;        
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<RegistrationCacheService> _logger;

        private const string Prefix = Constants.CacheIdentifiers.LinkagePrefix;

        public RegistrationCacheService(
            IConnectionMultiplexerFactory connectionMultiplexerFactory,
            IOptions<ConfigurationSettings> settings,
            ILogger<RegistrationCacheService> logger,
            IHashingService hashingService)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory ??
                                            throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            _settings = settings.Value;
            _hashingService = hashingService;

            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            _logger = logger;
        }
        
        public async Task<string> CreateRegistrationToken<T>(string key, T value)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            var sessionExpirationTime = TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes);
            RedisValue registrationGuid = JsonConvert.SerializeObject(value, _serializerSettings);

            RedisKey registrationGuidKey = Prefix + _hashingService.Hash(key);

            await database.StringSetAsync( registrationGuidKey, registrationGuid, sessionExpirationTime);
            return registrationGuidKey;
            
        }

        public async Task<Option<T>> GetRegistrationToken<T>(string key)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            RedisKey redisKey = Prefix + _hashingService.Hash(key);
            var redisValue = await database.StringGetWithExpiryAsync(redisKey);
            if (redisValue.Value.IsNull)
            {
                _logger.LogDebug("No redis value Found");
                return Option.None<T>();
            }

            var registrationGuid = JsonConvert.DeserializeObject<T>(redisValue.Value);

            return Option.Some(registrationGuid);
        }

        public async Task<bool> DeleteRegistrationToken(string key)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.Session);
            var database = multiplexer.GetDatabase();
            RedisKey redisKey = Prefix + _hashingService.Hash(key);
            
            return await database.KeyDeleteAsync(redisKey);
        }        
    }
}