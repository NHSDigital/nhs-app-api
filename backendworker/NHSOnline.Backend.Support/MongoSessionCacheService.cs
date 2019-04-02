using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NHSOnline.Backend.Support.Cipher;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Support
{
    public interface IMongoSessionCacheServiceConfig
    {
        string MongoDatabaseName { get; }
        string MongoDatabaseIm1CollectionName { get; }
    }

    public class MongoSessionCacheServiceConfig : IMongoSessionCacheServiceConfig
    {
        public string MongoDatabaseName { get; }
        public string MongoDatabaseIm1CollectionName { get; }

        public MongoSessionCacheServiceConfig(IConfiguration configuration, ILogger<MongoSessionCacheServiceConfig> logger)
        {
            MongoDatabaseName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            MongoDatabaseIm1CollectionName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_COLLECTION", logger);
        }
    }
    
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(UserSession userSession);
        Task<Option<UserSession>> GetUserSession(string sessionId);
        Task<bool> DeleteUserSession(string sessionId);
        Task UpdateUserSession(UserSession userSession);
    }

    public class MongoSessionCacheService : ISessionCacheService
    {
        private readonly string _databaseName;
        private readonly string _collectionName;

        private readonly IMongoClient _mongoClient;
        private readonly ICipherService _cipherService;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger<MongoSessionCacheService> _logger;
        
        public MongoSessionCacheService(
            ICipherService cipherService,
            ILogger<MongoSessionCacheService> logger,
            IMongoClient mongoClient,
            IMongoSessionCacheServiceConfig config
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

        public async Task<string> CreateUserSession(UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);
                sessionObject = _cipherService.Encrypt(sessionObject);
                var sessionKey = Guid.NewGuid().ToString();
                var update = 
                    new BsonDocument{GetId(sessionKey), GetSession(sessionObject), GetCurrentTimestamp()};

                using (_logger.WithTimer("Add session to Mongo"))
                {
                    await GetCollection().InsertOneAsync(update);
                }

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

                var filter = new BsonDocument(GetId(sessionId));
                var update = new BsonDocument( "$set", new BsonDocument(GetCurrentTimestamp()) );

                BsonDocument sessionValue;
                using (_logger.WithTimer("Get session from Mongo"))
                {
                    sessionValue = await GetCollection().FindOneAndUpdateAsync(filter,update);
                }
                if (sessionValue == null)
                {
                    _logger.LogDebug("No Mongo value Found");
                    return Option.None<UserSession>();
                }
                
                var userSession = JsonConvert
                    .DeserializeObject<UserSession>(_cipherService.Decrypt(sessionValue["session"].ToString()), _serializerSettings);

                userSession.Key = sessionId;

                return Option.Some(userSession);
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

                var filter = new BsonDocument(GetId(sessionId));

                using (_logger.WithTimer("Delete session in Mongo"))
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

        public async Task UpdateUserSession(UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);
                sessionObject = _cipherService.Encrypt(sessionObject);
                var filter = new BsonDocument(GetId(userSession.Key));
                var update = new BsonDocument( "$set", new BsonDocument{GetSession(sessionObject), GetCurrentTimestamp()});

                using (_logger.WithTimer("Update Mongo session"))
                {
                    await GetCollection().UpdateOneAsync(filter, update);
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

        private static BsonElement GetId(string id)
        {
            return new BsonElement("_id", id);
        }
        
        private static BsonElement GetSession(string sessionObject)
        {
            return new BsonElement("session", sessionObject);
        }
        
        private static BsonElement GetCurrentTimestamp()
        {
            return new BsonElement("_ts", new BsonDateTime(DateTime.UtcNow));
        }
    }
}