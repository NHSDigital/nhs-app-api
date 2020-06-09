using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Repository
{
    public class MongoRepository<TConfig, TRecord> : IRepository<TRecord>
        where TRecord : RepositoryRecord
        where TConfig : IMongoConfiguration
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly string _databaseName;
        private readonly string _collectionName;

        public MongoRepository(IApiMongoClient<TConfig> mongoClient, TConfig mongoConfiguration,
            ILogger<MongoRepository<TConfig, TRecord>> logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _databaseName = mongoConfiguration.DatabaseName;
            _collectionName = mongoConfiguration.CollectionName;
        }

        public async Task<RepositoryCreateResult<TRecord>> Create(TRecord record, string recordName)
        {
            try
            {
                await CreateRecord(record, recordName);
                return new RepositoryCreateResult<TRecord>.Created(record);
            }
            catch (MongoException exception)
            {
                _logger.LogError(exception, $"Mongo Failure. Create {recordName}.");
                return new RepositoryCreateResult<TRecord>.RepositoryError();
            }
        }

        public async Task<RepositoryCreateResult<TRecord>> CreateOrUpdate(Expression<Func<TRecord, bool>> filter,
            TRecord record, string recordName)
        {
            try
            {
                var result = await CreateOrUpdateRecords(filter, record, recordName);
                if (result.IsAcknowledged)
                {
                    _logger.LogInformation($"Mongo Create or Update {recordName} Successful.");
                    return new RepositoryCreateResult<TRecord>.Created(record);
                }

                _logger.LogError(
                    $"Mongo Failure. Create or Update {recordName}. " +
                    $"ReplaceOneAsync with Upsert. " +
                    $"IsAcknowledged: {result.IsAcknowledged}");
                return new RepositoryCreateResult<TRecord>.RepositoryError();
            }
            catch (MongoException exception)
            {
                _logger.LogError(exception, $"Mongo Failure. Create or update {recordName}.");
                return new RepositoryCreateResult<TRecord>.RepositoryError();
            }
        }

        public async Task<RepositoryUpdateResult<TRecord>> Update(
            Expression<Func<TRecord, bool>> filter,
            UpdateRecordBuilder<TRecord> updates,
            string recordName)
        {
            try
            {
                var result = await UpdateRecords(filter, updates.Build(), recordName);
                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    _logger.LogInformation($"Mongo Update {recordName} Successful.");
                    return new RepositoryUpdateResult<TRecord>.Updated();
                }

                if (result.IsAcknowledged && result.MatchedCount <= 0)
                {
                    _logger.LogInformation($"Mongo Update {recordName} failed to find record.");
                    return new RepositoryUpdateResult<TRecord>.NotFound();
                }

                _logger.LogError(
                    $"Mongo Update {recordName} Failed. " +
                    $"IsAcknowledged: {result.IsAcknowledged}, " +
                    $"ModifiedCount: {result.ModifiedCount}, " +
                    $"MatchedCount: {result.MatchedCount}");
                return new RepositoryUpdateResult<TRecord>.NoChange();
            }
            catch (MongoException exception)
            {
                _logger.LogError(exception, $"Mongo Failure. Update {recordName}");
                return new RepositoryUpdateResult<TRecord>.RepositoryError();
            }
        }

        public async Task<RepositoryFindResult<TRecord>> Find(Expression<Func<TRecord, bool>> filter, string recordName)
        {
            try
            {
                var records = await FindRecords(filter, recordName);

                if (records.Any())
                {
                    _logger.LogInformation($"Mongo Find {recordName} Successful. " +
                                           $"Count: {records.Count}");
                    return new RepositoryFindResult<TRecord>.Found(records);
                }

                _logger.LogInformation($"Mongo Find {recordName} Successful. " +
                                       "No records found");
                return new RepositoryFindResult<TRecord>.NotFound();
            }
            catch (MongoException exception)
            {
                _logger.LogError(exception, $"Mongo Failure. Find {recordName}.");
                return new RepositoryFindResult<TRecord>.RepositoryError();
            }
        }

        public async Task<RepositoryDeleteResult<TRecord>> Delete(Expression<Func<TRecord, bool>> filter,
            string recordName)
        {
            try
            {
                var result = await DeleteRecords(filter, recordName);

                if (result.IsAcknowledged && result.DeletedCount > 0)
                {
                    _logger.LogInformation($"Mongo Delete {recordName} Successful. " +
                                           $"DeletedCount: {result.DeletedCount}");
                    return new RepositoryDeleteResult<TRecord>.Deleted();
                }

                if (result.IsAcknowledged && result.DeletedCount == 0)
                {
                    _logger.LogInformation($"Mongo Delete {recordName} failed to find record.");
                    return new RepositoryDeleteResult<TRecord>.NotFound();
                }


                _logger.LogError(
                    $"Mongo Failure. Delete {recordName}. " +
                    $"IsAcknowledged: {result.IsAcknowledged}");

                return new RepositoryDeleteResult<TRecord>.RepositoryError();
            }
            catch (MongoException exception)
            {
                _logger.LogError(exception, $"Mongo Failure. Delete {recordName}.");
                return new RepositoryDeleteResult<TRecord>.RepositoryError();
            }
        }

        private async Task CreateRecord(TRecord record, string recordName)
        {
            using (_logger.WithTimer($"Mongo Create {recordName}."))
            {
                record.Timestamp = DateTime.UtcNow;
                await GetCollection().InsertOneAsync(record);
            }
        }

        private async Task<ReplaceOneResult> CreateOrUpdateRecords(Expression<Func<TRecord, bool>> filter,
            TRecord record, string recordName)
        {
            using (_logger.WithTimer($"Mongo Create Or Update {recordName}."))
            {
                record.Timestamp = DateTime.UtcNow;
                return await GetCollection().ReplaceOneAsync(filter, record, new ReplaceOptions { IsUpsert = true });
            }
        }

        private async Task<DeleteResult> DeleteRecords(Expression<Func<TRecord, bool>> filter, string recordName)
        {
            using (_logger.WithTimer($"Mongo Delete {recordName}."))
            {
                return await GetCollection().DeleteOneAsync(filter);
            }
        }

        private async Task<List<TRecord>> FindRecords(Expression<Func<TRecord, bool>> filter, string recordName)
        {
            using (_logger.WithTimer($"Mongo Find {recordName}."))
            {
                var records = await GetCollection().FindAsync(filter);
                return records.ToList();
            }
        }

        private async Task<UpdateResult> UpdateRecords(Expression<Func<TRecord, bool>> filter,
            UpdateDefinition<TRecord> updates,
            string recordName)
        {
            using (_logger.WithTimer($"Mongo Update {recordName}."))
            {
                return await GetCollection().UpdateManyAsync(filter, updates);
            }
        }

        private IMongoCollection<TRecord> GetCollection()
            => _mongoClient.GetDatabase(_databaseName).GetCollection<TRecord>(_collectionName);
    }
}