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
        where TConfig : IRepositoryConfiguration
    {
        private readonly IMongoClientService _mongoClientService;
        private readonly TConfig _mongoConfig;
        private readonly ILogger _logger;
        private const int DefaultListSize = 4;

        public MongoRepository(
            IMongoClientService mongoClientService,
            TConfig mongoConfiguration,
            ILogger<MongoRepository<TConfig, TRecord>> logger)
        {
            _mongoClientService = mongoClientService;
            _mongoConfig = mongoConfiguration;
            _logger = logger;
        }

        public async Task<RepositoryCountResult> Count(Expression<Func<TRecord, bool>> filter, string recordName)
        {
            try
            {
                var count = await CountRecords(filter, recordName);
                return new RepositoryCountResult.Found(count);
            }
            catch (MongoException exception)
            {
                _logger.LogError(exception, $"Mongo Failure. Count {recordName}.");
                return new RepositoryCountResult.RepositoryError();
            }
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
                return new RepositoryCreateResult<TRecord>.RepositoryError(exception);
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

        public async Task<RepositoryFindResult<TRecord>> Find(Expression<Func<TRecord, bool>> filter, string recordName, int? maxRecords = null)
        {
            try
            {
                var cursor = await FindRecords(filter, recordName, maxRecords);

                if (!await cursor.MoveNextAsync() || !cursor.Current.Any())
                {
                    _logger.LogInformation($"Mongo Find {recordName} Successful. " +
                                           "No records found");

                    cursor.Dispose();
                    return new RepositoryFindResult<TRecord>.NotFound();
                }

                _logger.LogInformation($"Mongo Find {recordName} Successful. " +
                                       $"One or more records found");

                var records = new List<TRecord>(maxRecords ?? DefaultListSize);
                await foreach (var record in EnumerateResults(cursor))
                {
                    records.Add(record);
                }

                return new RepositoryFindResult<TRecord>.Found(records);
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

        private async IAsyncEnumerable<TRecord> EnumerateResults(IAsyncCursor<TRecord> records)
        {
            using (records)
            {
                do
                {
                    foreach (var record in records.Current)
                    {
                        yield return record;
                    }
                } while (await records.MoveNextAsync());
            }
        }

        private async Task<long> CountRecords(Expression<Func<TRecord, bool>> filter, string recordName)
        {
            using (_logger.WithTimer($"Mongo Find {recordName}."))
            {
                return await _mongoClientService.CountAsync(_mongoConfig, filter);
            }
        }

        private async Task CreateRecord(TRecord record, string recordName)
        {
            using (_logger.WithTimer($"Mongo Create {recordName}."))
            {
                record.Timestamp = DateTime.UtcNow;
                await _mongoClientService.InsertOneAsync(_mongoConfig, record);
            }
        }

        private async Task<ReplaceOneResult> CreateOrUpdateRecords(Expression<Func<TRecord, bool>> filter,
            TRecord record, string recordName)
        {
            using (_logger.WithTimer($"Mongo Create Or Update {recordName}."))
            {
                record.Timestamp = DateTime.UtcNow;
                return await _mongoClientService.ReplaceOneAsync(_mongoConfig, filter, record, new ReplaceOptions { IsUpsert = true });
            }
        }

        private async Task<DeleteResult> DeleteRecords(Expression<Func<TRecord, bool>> filter, string recordName)
        {
            using (_logger.WithTimer($"Mongo Delete {recordName}."))
            {
                return await _mongoClientService.DeleteOneAsync(_mongoConfig, filter);
            }
        }

        private async Task<IAsyncCursor<TRecord>> FindRecords(Expression<Func<TRecord, bool>> filter, string recordName, int? maxRecords)
        {
            using (_logger.WithTimer($"Mongo Find {recordName}."))
            {
                return await _mongoClientService.FindAsync(_mongoConfig, filter, new FindOptions<TRecord> { Limit = maxRecords });
            }
        }

        private async Task<UpdateResult> UpdateRecords(Expression<Func<TRecord, bool>> filter,
            UpdateDefinition<TRecord> updates,
            string recordName)
        {
            using (_logger.WithTimer($"Mongo Update {recordName}."))
            {
                return await _mongoClientService.UpdateManyAsync(_mongoConfig, filter, updates);
            }
        }
    }
}
