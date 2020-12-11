using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal sealed class MongoIm1CacheAccessor
    {
        private const string IdElementName = "_id";
        private const string ConnectionTokenElementName = "token";
        private const string DocumentTypeElementName = "doctype";
        private const string DocumentTypeElementValue = "im1connectiontoken";

        private readonly ILogger _logger;
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

        internal MongoIm1CacheAccessor(
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

        internal async Task Save(string key, string token)
        {
            try
            {
                _logger.LogEnter();

                var id = Id(key);
                var filter = new BsonDocument(id);
                var document = new BsonDocument { id, ConnectionToken(token), DocumentType() };

                using (_logger.WithTimer("Add IM1 connection token to cache"))
                {
                    await GetCollection().ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true });
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Option<string>> Get(string key)
        {
            try
            {
                _logger.LogEnter();

                var filter = new BsonDocument(Id(key));

                BsonDocument sessionValue;
                using (_logger.WithTimer("Get IM1 connection token from cache"))
                {
                    sessionValue = await GetCollection().Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
                }

                if (sessionValue == null)
                {
                    _logger.LogDebug("No IM1 connection token value found in cache");
                    return Option.None<string>();
                }

                var token = sessionValue[ConnectionTokenElementName].ToString();

                return Option.Some(token);
            }
            finally
            {
                _logger.LogExit();
            }
        }


        public async Task<bool> Delete(string key)
        {
            try
            {
                _logger.LogEnter();

                var filter = new BsonDocument(Id(key));

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

        private static BsonElement Id(string id) => new BsonElement(
            IdElementName, id);

        private static BsonElement ConnectionToken(string connectionToken) => new BsonElement(
            ConnectionTokenElementName,
            connectionToken);

        private static BsonElement DocumentType() => new BsonElement(
            DocumentTypeElementName,
            DocumentTypeElementValue);
    }
}