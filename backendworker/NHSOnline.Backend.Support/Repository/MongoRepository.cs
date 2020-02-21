using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace NHSOnline.Backend.Support.Repository
{
    public abstract class MongoRepository<TConfig, TRecord>
        where TRecord : MongoRecord
        where TConfig : IMongoConfiguration
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

        protected MongoRepository(IApiMongoClient<TConfig> mongoClient, TConfig mongoConfiguration)
        {
            _mongoClient = mongoClient;
            _databaseName = mongoConfiguration.DatabaseName;
            _collectionName = mongoConfiguration.CollectionName;
        }

        protected async Task InsertOne(TRecord record)
        {
            record.Timestamp = DateTime.UtcNow;
            await GetCollection().InsertOneAsync(record);
        }

        protected async Task CreateOrUpdateOne(Expression<Func<TRecord, bool>> filter, TRecord record)
        {
            record.Timestamp = DateTime.UtcNow;
            await GetCollection().ReplaceOneAsync(filter, record, new UpdateOptions { IsUpsert = true });
        }

        protected async Task DeleteOne(Expression<Func<TRecord, bool>> filter)
            => await GetCollection().DeleteOneAsync(filter);

        protected async Task<TRecord> FindOne(Expression<Func<TRecord, bool>> filter)
        {
            var records = await Find(filter);
            return records.FirstOrDefault();
        }

        protected async Task<IEnumerable<TRecord>> Find(Expression<Func<TRecord, bool>> filter)
        {
            var records = await GetCollection().FindAsync(filter);
            return records.ToEnumerable();
        }

        protected async Task UpdateOne(Expression<Func<TRecord, bool>> filter, TRecord record)
            => await GetCollection().ReplaceOneAsync(filter, record);

        private IMongoCollection<TRecord> GetCollection()
            => _mongoClient.GetDatabase(_databaseName).GetCollection<TRecord>(_collectionName);
    }
}