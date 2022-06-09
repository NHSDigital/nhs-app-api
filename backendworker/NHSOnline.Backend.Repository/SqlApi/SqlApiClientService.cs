using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public class SqlApiClientService : ISqlApiClientService
    {
        private readonly ICosmosClientWrapper _cosmosClientWrapper;

        public SqlApiClientService(ICosmosClientWrapper cosmosClientWrapper)
        {
            _cosmosClientWrapper = cosmosClientWrapper;
        }

        public async Task<ItemResponse<TRecord>> UpsertOneAsync<TRecord>(ISqlApiRepositoryConfiguration config,
            TRecord record, string partitionKeyValue)
            where TRecord : RepositoryRecord
        {
            return await GetContainer(config).UpsertItemAsync<TRecord>(record, new PartitionKey(partitionKeyValue));
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