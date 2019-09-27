using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace NHSOnline.Backend.Support.Repository
{
    public abstract class MongoRepository<TRecord>
        where TRecord : MongoRecord
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

        protected MongoRepository(IMongoClient mongoClient, IMongoConfiguration mongoConfiguration)
        {
            _mongoClient = mongoClient;
            _databaseName = mongoConfiguration.DatabaseName;
            _collectionName = mongoConfiguration.CollectionName;
        }

        protected async Task InsertOneAsync(TRecord record)
        {
            record.TimeStamp = DateTime.UtcNow;
            await GetCollection().InsertOneAsync(record);
        }

        protected async Task<TRecord> FindOneAsync(Expression<Func<TRecord, bool>> filter)
        {
            var records = await GetCollection().FindAsync(filter);
            return records.FirstOrDefault();
        }

        protected async Task DeleteOneAsync(Expression<Func<TRecord, bool>> filter)
            => await GetCollection().DeleteOneAsync(filter);

        private IMongoCollection<TRecord> GetCollection()
            => _mongoClient.GetDatabase(_databaseName).GetCollection<TRecord>(_collectionName);
    }
}