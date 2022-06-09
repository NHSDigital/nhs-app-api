using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public class SqlApiRepository<TConfig, TRecord> : ISqlApiRepository<TRecord>
        where TConfig : ISqlApiRepositoryConfiguration
        where TRecord : RepositoryRecord
    {
        private readonly ISqlApiClientService _sqlApiClientService;
        private readonly TConfig _config;
        private readonly ILogger _logger;

        public SqlApiRepository(
            ISqlApiClientService sqlApiClientService,
            TConfig config,
            ILogger<SqlApiRepository<TConfig, TRecord>> logger)
        {
            _sqlApiClientService = sqlApiClientService;
            _config = config;
            _logger = logger;
        }

        public async Task<RepositoryCreateResult<TRecord>> CreateOrUpdate(TRecord record, string recordName,
            string partitionKeyValue)
        {
            try
            {
                var result = await CreateOrUpdateRecord(record, recordName, partitionKeyValue);
                if (result.StatusCode == HttpStatusCode.Created || result.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Cosmos Sql Api Create or Update {recordName} Successful.");
                    return new RepositoryCreateResult<TRecord>.Created(result.Resource);
                }

                _logger.LogError(
                    $"Cosmos Sql Api Failure. Create or Update {recordName}. " +
                    $"StatusCode: {result.StatusCode}");

                return new RepositoryCreateResult<TRecord>.RepositoryError();
            }
            catch (ArgumentNullException nullException)
            {
                _logger.LogError(nullException,
                    $"Cosmos Sql Api Failure due to null value. Create or update {recordName}.");
                return new RepositoryCreateResult<TRecord>.RepositoryError();
            }
            catch (CosmosException cosmosException)
            {
                _logger.LogError(cosmosException, $"Cosmos Sql Api Failure. Create or update {recordName}." +
                                                  $"HttpStatusCode: {cosmosException.StatusCode}, subStatusCode: {cosmosException.SubStatusCode}");
                return new RepositoryCreateResult<TRecord>.RepositoryError();
            }
        }

        public async Task<RepositoryDeleteResult<TRecord>> Delete(string id, string partitionKeyValue,
            string recordName)
        {
            try
            {
                var result = await DeleteRecord(id, partitionKeyValue, recordName);

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Cosmos Sql Api Delete {recordName} Successful.");
                    return new RepositoryDeleteResult<TRecord>.Deleted();
                }

                _logger.LogError($"Cosmos Sql Api Failure. Delete {recordName}. " +
                                 $"Status code: {result.StatusCode}");
                return new RepositoryDeleteResult<TRecord>.RepositoryError();
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation($"Cosmos Sql Api Delete {recordName} failed to find record.");
                    return new RepositoryDeleteResult<TRecord>.NotFound();
                }

                _logger.LogError(cosmosException, $"Cosmos Sql Api Failure. Delete {recordName}." +
                                                  $"HttpStatusCode: {cosmosException.StatusCode}, subStatusCode: {cosmosException.SubStatusCode}");
                return new RepositoryDeleteResult<TRecord>.RepositoryError();
            }
        }

        public async Task<RepositoryFindResult<TRecord>> Find(string id, string partitionKeyValue, string recordName)
        {
            try
            {
                var result = await FindRecord(id, partitionKeyValue, recordName);
                _logger.LogInformation($"Cosmos Sql Api Find {recordName} successful.");

                return new RepositoryFindResult<TRecord>.Found(new[] { result.Resource });
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation(
                        $"Cosmos Sql Api {recordName} failed to find record and returned cosmos exception.");

                    return new RepositoryFindResult<TRecord>.NotFound();
                }

                _logger.LogError(cosmosException, $"Cosmos Sql Api Failure. Find {recordName}." +
                                                  $"HttpStatusCode: {cosmosException.StatusCode}, subStatusCode: {cosmosException.SubStatusCode}");

                return new RepositoryFindResult<TRecord>.RepositoryError();
            }
        }

        private async Task<ItemResponse<TRecord>> CreateOrUpdateRecord(TRecord record, string recordName,
            string partitionKeyValue)
        {
            using (_logger.WithTimer($"CosmosDb Sql Api Create Or Update {recordName}."))
            {
                record.Timestamp = DateTime.UtcNow;
                return await _sqlApiClientService.UpsertOneAsync(_config, record, partitionKeyValue);
            }
        }

        private async Task<ItemResponse<TRecord>> DeleteRecord(
            string id, string partitionKeyValue, string recordName)
        {
            using (_logger.WithTimer($"CosmosDb Sql Api Delete {recordName}."))
            {
                return await _sqlApiClientService.DeleteOneAsync<TRecord>(_config, id, partitionKeyValue);
            }
        }

        private async Task<ItemResponse<TRecord>> FindRecord(
            string id, string partitionKeyValue, string recordName)
        {
            using (_logger.WithTimer($"CosmosDb Sql Api Find {recordName}."))
            {
                return await _sqlApiClientService.FindOneAsync<TRecord>(_config, id, partitionKeyValue);
            }
        }
    }
}