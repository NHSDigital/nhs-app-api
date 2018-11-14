using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Support.Cipher;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Support.Session
{
    public class MongoSessionCacheService : ISessionCacheService
    {
        private static IMongoClient _mongoClient;
        private static string _sessionDbName;
        private static string _sessionDbCollection;
        
        private readonly ICipherService _cipherService;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger<MongoSessionCacheService> _logger;
        
        public MongoSessionCacheService(
            ICipherService cipherService,
            ILogger<MongoSessionCacheService> logger,
            IMongoSessionCacheServiceConfig configuration
            )
        {
            _sessionDbName = configuration.SessionMongoDatabaseName;
            _sessionDbCollection = configuration.SessionMongoDatabaseCollection;
            _mongoClient = BuildMongoClient(configuration);
            
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
                var collection = GetMongoCollection();

                var sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);
                sessionObject = _cipherService.Encrypt(sessionObject);
                var sessionKey = Guid.NewGuid().ToString();
                var update = 
                    new BsonDocument{GetId(sessionKey), GetSession(sessionObject), GetCurrentTimestamp()};

                using (_logger.WithTimer("Add session to Mongo"))
                {
                    await collection.InsertOneAsync(update);
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

                var collection = GetMongoCollection();
                var filter = new BsonDocument(GetId(sessionId));
                var update = new BsonDocument( "$set", new BsonDocument(GetCurrentTimestamp()) );

                BsonDocument sessionValue;
                using (_logger.WithTimer("Get session from Mongo"))
                {
                    sessionValue = await collection.FindOneAndUpdateAsync(filter,update);
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
                _logger.LogExit(nameof(GetUserSession));
            }
        }

        public async Task<bool> DeleteUserSession(string sessionId)
        {
            try
            {
                _logger.LogEnter(nameof(DeleteUserSession));

                var collection = GetMongoCollection();
                var filter = new BsonDocument(GetId(sessionId));


                using (_logger.WithTimer("Delete session in Mongo"))
                {
                    var result = await collection.DeleteOneAsync(filter);
                    return result.IsAcknowledged;
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
                var collection = GetMongoCollection();

                var sessionObject = JsonConvert.SerializeObject(userSession, _serializerSettings);
                sessionObject = _cipherService.Encrypt(sessionObject);
                var filter = new BsonDocument(GetId(userSession.Key));
                var update = new BsonDocument( "$set", new BsonDocument{GetSession(sessionObject), GetCurrentTimestamp()});

                using (_logger.WithTimer("Update Mongo session"))
                {
                    await collection.UpdateOneAsync(filter, update);
                }
            }
            finally
            {
                _logger.LogExit(nameof(UpdateUserSession));
            }
        }
        
        protected MongoClient BuildMongoClient(IMongoSessionCacheServiceConfig configuration)
        {
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(configuration.SessionMongoDatabaseHost,
                    configuration.SessionMongoDatabasePort)
            };

            if (!string.IsNullOrEmpty(configuration.SessionMongoDatabaseUsername))
            {
                settings.UseSsl = true;
                settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

                MongoIdentity identity = new MongoInternalIdentity(_sessionDbName, configuration.SessionMongoDatabaseUsername);
                MongoIdentityEvidence evidence = new PasswordEvidence(configuration.SessionMongoDatabasePassword);
                settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
            }

            return new MongoClient(settings);
        }

        private static IMongoCollection<BsonDocument> GetMongoCollection()
        {
            return _mongoClient.GetDatabase(_sessionDbName).GetCollection<BsonDocument>(_sessionDbCollection);
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