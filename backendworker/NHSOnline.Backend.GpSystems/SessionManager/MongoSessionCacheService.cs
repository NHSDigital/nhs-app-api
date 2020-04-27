using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Cipher;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
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
            IMongoSessionCacheServiceConfig config)
        {
            _mongoClient = mongoClient;

            _cipherService = cipherService;
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                SerializationBinder = new RenameUserSessionSerializationBinder(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            _logger = logger;

            _databaseName = config.MongoDatabaseName;
            _collectionName = config.MongoDatabaseSessionCollectionName;
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

                var sessionJson = _cipherService.Decrypt(sessionValue["session"].ToString());
                var userSession = JsonConvert.DeserializeObject<UserSession>(sessionJson, _serializerSettings);

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

        private sealed class RenameUserSessionSerializationBinder : ISerializationBinder
        {
            private readonly DefaultSerializationBinder _defaultBinder = new DefaultSerializationBinder();

            public Type BindToType(string assemblyName, string typeName)
            {
                switch (typeName)
                {
                    case "NHSOnline.Backend.Support.P9UserSession":
                    case "P9UserSession":
                        return typeof(P9UserSession);

                    case "NHSOnline.Backend.Support.Session.P5UserSession":
                    case "P5UserSession":
                        return typeof(P5UserSession);

                    default:
                        return _defaultBinder.BindToType(assemblyName, typeName);
                }
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
                => _defaultBinder.BindToName(serializedType, out assemblyName, out typeName);
        }
    }
}