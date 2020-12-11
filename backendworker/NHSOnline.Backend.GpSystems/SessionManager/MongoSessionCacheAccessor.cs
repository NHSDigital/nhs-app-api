using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal sealed class MongoSessionCacheAccessor
    {
        private readonly ILogger _logger;
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

        internal MongoSessionCacheAccessor(
            ILogger logger,
            IMongoClient mongoClient,
            string databaseName,
            string collectionName)
        {
            _logger = logger;
            _mongoClient = mongoClient;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        public async Task<string> Create(string encodedUserSession)
        {
            try
            {
                _logger.LogEnter();

                var sessionKey = Guid.NewGuid().ToString();
                var update = new BsonDocument
                {
                    Id(sessionKey),
                    Session(encodedUserSession),
                    CurrentTimestamp()
                };

                using (_logger.WithTimer("Add session to Mongo"))
                {
                    await GetCollection().InsertOneAsync(update);
                }

                return sessionKey;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Option<string>> Get(string sessionId)
        {
            try
            {
                _logger.LogEnter();

                var filter = new BsonDocument(Id(sessionId));
                var update = new BsonDocument("$set", new BsonDocument(CurrentTimestamp()));

                BsonDocument sessionValue;
                using (_logger.WithTimer("Get session from Mongo"))
                {
                    sessionValue = await GetCollection().FindOneAndUpdateAsync(filter, update);
                }
                if (sessionValue == null)
                {
                    _logger.LogDebug("No Mongo value Found");
                    return Option.None<string>();
                }

                return Option.Some(sessionValue["session"].ToString());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<bool> Delete(string sessionId)
        {
            try
            {
                _logger.LogEnter();

                var filter = new BsonDocument(Id(sessionId));

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

        public async Task Update(string key, string encodedUserSession)
        {
            try
            {
                _logger.LogEnter();

                var filter = new BsonDocument(Id(key));
                var update = new BsonDocument("$set", new BsonDocument { Session(encodedUserSession), CurrentTimestamp() });

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

        private static BsonElement Id(string id)
        {
            return new BsonElement("_id", id);
        }

        private static BsonElement Session(string sessionObject)
        {
            return new BsonElement("session", sessionObject);
        }

        private static BsonElement CurrentTimestamp()
        {
            return new BsonElement("_ts", new BsonDateTime(DateTime.UtcNow));
        }
    }
}