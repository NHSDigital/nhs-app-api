using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public class SqlApiClientService : ISqlApiClientService
    {
        private readonly ICosmosClientWrapper _cosmosClientWrapper;
        private readonly ICosmosLinqQuery _cosmosLinqQuery;

        public SqlApiClientService(ICosmosClientWrapper cosmosClientWrapper, ICosmosLinqQuery cosmosLinqQuery)
        {
            _cosmosClientWrapper = cosmosClientWrapper;
            _cosmosLinqQuery = cosmosLinqQuery;
        }

        public async Task<ItemResponse<TRecord>> UpsertOneAsync<TRecord>(ISqlApiRepositoryConfiguration config,
            TRecord record, string partitionKeyValue)
            where TRecord : RepositoryRecord
        {
            return await GetContainer(config).UpsertItemAsync(record, new PartitionKey(partitionKeyValue));
        }

        public async Task<ItemResponse<TRecord>> DeleteOneAsync<TRecord>(ISqlApiRepositoryConfiguration config,
            string id, string partitionKeyValue)
            where TRecord : RepositoryRecord
        {
            return await GetContainer(config).DeleteItemAsync<TRecord>(id, new PartitionKey(partitionKeyValue));
        }

        public async Task<ItemResponse<TRecord>> FindOneAsync<TRecord>(ISqlApiRepositoryConfiguration config, string id,
            string partitionKeyValue) where TRecord : RepositoryRecord
        {
            return await GetContainer(config).ReadItemAsync<TRecord>(id, new PartitionKey(partitionKeyValue));
        }

        public async Task<List<FeedResponse<TRecord>>> FindAsync<TRecord>(ISqlApiRepositoryConfiguration config,
            Expression<Func<TRecord, bool>> filter, string partitionKeyValue) where TRecord : RepositoryRecord
        {
            var queryableResultSet = GetContainer(config).GetItemLinqQueryable<TRecord>(false, null,
                new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKeyValue) }).Where(filter);

            return await GetRecords(queryableResultSet);
        }

        public async Task<List<FeedResponse<TRecord>>> FindQueryableAsync<TRecord>(
            ISqlApiRepositoryConfiguration config,
            Func<IQueryable<TRecord>, IQueryable<TRecord>> query) where TRecord : RepositoryRecord
        {
            var queryableResultSet = query
            (
                GetContainer(config)
                    .GetItemLinqQueryable<TRecord>(
                        false,
                        null,
                        new QueryRequestOptions())
            );

            return await GetRecords(queryableResultSet);
        }

        private async Task<List<FeedResponse<TRecord>>> GetRecords<TRecord>(IQueryable<TRecord> queryableResultSet)
            where TRecord : RepositoryRecord
        {
            var feedIterator = _cosmosLinqQuery.GetFeedIterator(queryableResultSet);

            var records = new List<FeedResponse<TRecord>>();

            while (feedIterator.HasMoreResults)
            {
                records.Add(await feedIterator.ReadNextAsync());
            }

            return records;
        }

        public async Task<ContainerResponse> CheckHealthAsync(ISqlApiRepositoryConfiguration config)
        {
            return await GetContainer(config).ReadContainerAsync();
        }

        private Container GetContainer(ISqlApiRepositoryConfiguration config)
        {
            return _cosmosClientWrapper.GetContainer(config.DatabaseName, config.ContainerName);
        }
    }
}