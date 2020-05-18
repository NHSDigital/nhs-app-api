using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace NHSOnline.Backend.Support.Repository
{
    public abstract class MongoRepositoryBase<TConfig, TRecord>
        where TRecord : MongoRecord
        where TConfig : IMongoConfiguration
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly string _databaseName;
        private readonly string _collectionName;

        protected MongoRepositoryBase(IApiMongoClient<TConfig> mongoClient, TConfig mongoConfiguration, ILogger logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _databaseName = mongoConfiguration.DatabaseName;
            _collectionName = mongoConfiguration.CollectionName;
        }

        protected async Task<RepositoryCreateResult<TRecord>> CreateOrUpdateOne(Expression<Func<TRecord, bool>> filter, TRecord record)
        {
            try
            {
                record.Timestamp = DateTime.UtcNow;
                var result = await GetCollection().ReplaceOneAsync(filter, record, new ReplaceOptions { IsUpsert = true });
                if (result.IsAcknowledged)
                {
                    _logger.LogInformation("Mongo Create Successful.");
                    return new RepositoryCreateResult<TRecord>.Created(record);
                }
                _logger.LogError($"Mongo Failure. ReplaceOneAsync with Upsert. IsAcknowledged: {result.IsAcknowledged}");
                return new RepositoryCreateResult<TRecord>.InternalServerError();
            }
            catch (MongoException exception)
            {
                _logger.LogError($"Mongo Failure. Exception Caught: {exception}");
                return new RepositoryCreateResult<TRecord>.RepositoryError(exception);
            }
        }

        protected async Task<RepositoryFindResult<TRecord>> Find(Expression<Func<TRecord, bool>> filter)
        {
            try
            {
                var records = await GetCollection().FindAsync(filter);
                var recordList = records.ToList();
                if (recordList.Any())
                {
                    return new RepositoryFindResult<TRecord>.Found(recordList);
                }
                return new RepositoryFindResult<TRecord>.NotFound();
            }
            catch (MongoException exception)
            {
                _logger.LogError($"Mongo Failure. Exception Caught: {exception}");
                return new RepositoryFindResult<TRecord>.RepositoryError(exception);
            }
        }

        protected async Task<RepositoryUpdateResult<TRecord>> UpdateOne(
            Expression<Func<TRecord, bool>> filter,
            string path,
            string newValue)
        {
            try
            {
                var update = Builders<TRecord>.Update.Set(path, newValue);
                var result = await GetCollection().UpdateOneAsync(filter, update);
                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    _logger.LogInformation("Mongo Update Successful.");
                    return new RepositoryUpdateResult<TRecord>.Updated();
                }
                if (result.IsAcknowledged && result.MatchedCount <= 0)
                {
                    _logger.LogInformation("Mongo Update Failed to find record.");
                    return new RepositoryUpdateResult<TRecord>.NotFound();
                }

                _logger.LogError(
                    "Mongo Update Failed. " +
                    $"IsAcknowledged: {result.IsAcknowledged}, " +
                    $"ModifiedCount: {result.ModifiedCount}, " +
                    $"MatchedCount: {result.MatchedCount}");
                return new RepositoryUpdateResult<TRecord>.InternalServerError();
            }
            catch (MongoException exception)
            {
                _logger.LogError($"Mongo Failure. Exception Caught: {exception}");
                return new RepositoryUpdateResult<TRecord>.RepositoryError(exception);
            }
        }

        private IMongoCollection<TRecord> GetCollection()
            => _mongoClient.GetDatabase(_databaseName).GetCollection<TRecord>(_collectionName);
    }
}