using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Repository
{
    public class LocalMongoService: IMongoClientService
    {
        private IMongoClient _mongoClient;
        private readonly ILogger<LocalMongoService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMongoClientCreator _mongoClientCreator;

        public LocalMongoService(
            ILogger<LocalMongoService> logger,
            IConfiguration configuration,
            IMongoClientCreator mongoClientCreator)
        {
            _logger = logger;
            _configuration = configuration;
            _mongoClientCreator = mongoClientCreator;
            BuildClients();
        }

        private void BuildClients()
        {
            var connectionString = _configuration.GetOrThrow("MONGO_CONNECTION_STRING", _logger);

            _logger.LogInformation("Creating local mongo client");
            _mongoClient = _mongoClientCreator.CreatePrimaryMongoClient(connectionString);
            _logger.LogInformation("Local mongo client created");
        }

        public async Task CheckHealthAsync(string databaseName)
        {
            using var cursor = await _mongoClient
                .GetDatabase(databaseName)
                .ListCollectionNamesAsync();
            await cursor.FirstOrDefaultAsync();
        }

        public Task RebuildIfNecessary(string databaseName, Guid identifier, uint counter)
        {
            throw new NotImplementedException();
        }

        public bool SupportsConnectionRecovery => false;

        public async Task InsertOneAsync<TRecord>(IRepositoryConfiguration config, TRecord record) where TRecord : RepositoryRecord
        {
            await GetCollection<TRecord>(config).InsertOneAsync(record);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, TRecord record,
            ReplaceOptions replaceOptions) where TRecord : RepositoryRecord
        {
            return await GetCollection<TRecord>(config).ReplaceOneAsync(filter, record, replaceOptions);
        }

        public async Task<DeleteResult> DeleteOneAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter) where TRecord : RepositoryRecord
        {
            return await GetCollection<TRecord>(config).DeleteOneAsync(filter);
        }

        public async Task<IAsyncCursor<TRecord>> FindAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, FindOptions<TRecord> findOptions) where TRecord : RepositoryRecord
        {
            return await GetCollection<TRecord>(config).FindAsync(filter, findOptions);
        }

        public async Task<UpdateResult> UpdateManyAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, UpdateDefinition<TRecord> updates) where TRecord : RepositoryRecord
        {
            return await GetCollection<TRecord>(config).UpdateManyAsync(filter, updates);
        }

        public async Task<long> CountAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter) where TRecord : RepositoryRecord
        {
            return await GetCollection<TRecord>(config).CountDocumentsAsync(filter);
        }

        public async Task InsertOneDocumentAsync(IRepositoryConfiguration config, BsonDocument record)
        {
            await GetCollection<BsonDocument>(config).InsertOneAsync(record);
        }

        public async Task<UpdateResult> UpdateOneDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return await GetCollection<BsonDocument>(config).UpdateOneAsync(filter, update);
        }

        public async Task<BsonDocument> FindOneAndUpdateDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return await GetCollection<BsonDocument>(config).FindOneAndUpdateAsync(filter, update);
        }

        public async Task<BsonDocument> FindFirstDocument(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter)
        {
            return await GetCollection<BsonDocument>(config).Find(filter).FirstOrDefaultAsync();
        }

        public async Task<DeleteResult> DeleteOneDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter)
        {
            return await GetCollection<BsonDocument>(config).DeleteOneAsync(filter);
        }

        private IMongoCollection<TRecord> GetCollection<TRecord>(IRepositoryConfiguration config)
            => _mongoClient.GetDatabase(config.DatabaseName).GetCollection<TRecord>(config.CollectionName);
    }
}
