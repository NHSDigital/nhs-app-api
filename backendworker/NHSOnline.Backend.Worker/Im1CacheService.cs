using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Cipher;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker
{
    public interface IIm1CacheServiceConfig
    {
        string MongoDatabaseName { get; }
        string MongoDatabaseIm1CollectionName { get; }
    }

    public class Im1CacheServiceConfig : IIm1CacheServiceConfig
    {
        public string MongoDatabaseName { get; }
        public string MongoDatabaseIm1CollectionName { get; }

        public Im1CacheServiceConfig(IConfiguration configuration, ILogger<Im1CacheServiceConfig> logger)
        {
            MongoDatabaseName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            MongoDatabaseIm1CollectionName = configuration.GetOrThrow("IM1CACHE_MONGO_DATABASE_COLLECTION", logger);
        }
    }

    public interface IIm1CacheService
    {
        Task SaveIm1ConnectionToken<T>(string key, T value);
        Task<Option<T>> GetIm1ConnectionToken<T>(string key);
        Task<bool> DeleteIm1ConnectionToken(string key);
    }

    public class Im1CacheService : IIm1CacheService
    {
        private readonly IMongoClient _mongoClient;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger<Im1CacheService> _logger;
        private readonly ICipherService _cipherService;
        private readonly string _databaseName;
        private readonly string _collectionName;

        private const string IdElementName = "_id";
        private const string ConnectionTokenElementName = "token";
        private const string DocumentTypeElementName = "doctype";
        private const string DocumentTypeElementValue = "im1connectiontoken";
        public const string Im1ConnectionTokenCacheKeyPropertyName = "Im1CacheKey";

        public Im1CacheService(
            ICipherService cipherService,
            ILogger<Im1CacheService> logger,
            IMongoClient mongoClient,
            IIm1CacheServiceConfig config
            )
        {
            _mongoClient = mongoClient;
            _cipherService = cipherService;

            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            _logger = logger;

            _databaseName = config.MongoDatabaseName;
            _collectionName = config.MongoDatabaseIm1CollectionName;
        }

        public async Task SaveIm1ConnectionToken<T>(string key, T value)
        {
            try
            {
                _logger.LogEnter();

                var connectionToken = JsonConvert.SerializeObject(value, _serializerSettings);
                connectionToken = _cipherService.Encrypt(connectionToken);

                var id = GetId(key);
                var filter = new BsonDocument(id);
                var document = 
                    new BsonDocument{id, GetConnectionToken(connectionToken), GetDocumentType()};

                using (_logger.WithTimer("Add IM1 connection token to cache"))
                {
                    await GetCollection().ReplaceOneAsync(filter, document, new UpdateOptions { IsUpsert = true });
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Option<T>> GetIm1ConnectionToken<T>(string key)
        {
            try
            {
                _logger.LogEnter();

                var filter = new BsonDocument(GetId(key));

                BsonDocument sessionValue;
                using (_logger.WithTimer("Get IM1 connection token from cache"))
                {
                    sessionValue = await GetCollection().Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
                }
                if (sessionValue == null)
                {
                    _logger.LogDebug("No IM1 connection token value found in cache");
                    return Option.None<T>();
                }
                
                var connectionToken = JsonConvert
                    .DeserializeObject<T>(_cipherService.Decrypt(
                        sessionValue[ConnectionTokenElementName]
                            .ToString()));

                return Option.Some(connectionToken);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<bool> DeleteIm1ConnectionToken(string key)
        {
            try
            {
                _logger.LogEnter();

                var filter = new BsonDocument(GetId(key));

                using (_logger.WithTimer("Delete IM1 connection token from cache"))
                {
                    var result = await GetCollection().DeleteOneAsync(filter);
                    return result.IsAcknowledged;
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private IMongoCollection<BsonDocument> GetCollection()
        {
            return _mongoClient.GetDatabase(_databaseName).GetCollection<BsonDocument>(_collectionName);
        }

        private static BsonElement GetId(string id) => new BsonElement(
            IdElementName, id);

        private static BsonElement GetConnectionToken(string connectionToken) => new BsonElement(
            ConnectionTokenElementName,
            connectionToken);

        private static BsonElement GetDocumentType() => new BsonElement(
            DocumentTypeElementName,
            DocumentTypeElementValue);
    }
}