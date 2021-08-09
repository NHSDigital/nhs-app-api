using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NHSOnline.Backend.Repository
{
    public interface IMongoClientService
    {
        Task CheckHealthAsync(string databaseName);
        Task RebuildIfNecessary(string databaseName, Guid identifier, uint counter);
        bool SupportsConnectionRecovery { get; }

        Task InsertOneAsync<TRecord>(IRepositoryConfiguration config, TRecord record) where TRecord : RepositoryRecord;
        Task<ReplaceOneResult> ReplaceOneAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, TRecord record, ReplaceOptions replaceOptions) where TRecord : RepositoryRecord;
        Task<DeleteResult> DeleteOneAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter) where TRecord : RepositoryRecord;
        Task<IAsyncCursor<TRecord>> FindAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, FindOptions<TRecord> findOptions) where TRecord : RepositoryRecord;
        Task<UpdateResult> UpdateManyAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, UpdateDefinition<TRecord> updates) where TRecord : RepositoryRecord;


        Task InsertOneDocumentAsync(IRepositoryConfiguration config, BsonDocument record);
        Task<BsonDocument> FindOneAndUpdateDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update);
        Task<BsonDocument> FindFirstDocument(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter);
        Task<DeleteResult> DeleteOneDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter);
        Task<UpdateResult> UpdateOneDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update);
    }
}
