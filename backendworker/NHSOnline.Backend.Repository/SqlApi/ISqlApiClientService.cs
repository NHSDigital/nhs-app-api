using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public interface ISqlApiClientService
    {
        public Task<ItemResponse<TRecord>> UpsertOneAsync<TRecord>(
            ISqlApiRepositoryConfiguration config, TRecord record, string partitionKeyValue)
            where TRecord : RepositoryRecord;

        public Task<ItemResponse<TRecord>> DeleteOneAsync<TRecord>(
            ISqlApiRepositoryConfiguration config, string id, string partitionKeyValue)
            where TRecord : RepositoryRecord;

        public Task<ItemResponse<TRecord>> FindOneAsync<TRecord>(
            ISqlApiRepositoryConfiguration config, string id, string partitionKeyValue)
            where TRecord : RepositoryRecord;

        public Task<List<FeedResponse<TRecord>>> FindAsync<TRecord>(ISqlApiRepositoryConfiguration config,
            Expression<Func<TRecord, bool>> filter, string partitionKeyValue);

        public Task<ContainerResponse> CheckHealthAsync(ISqlApiRepositoryConfiguration config);
    }
}