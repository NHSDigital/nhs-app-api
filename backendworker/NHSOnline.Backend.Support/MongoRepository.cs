using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace NHSOnline.Backend.Support
{
    public abstract class MongoRepository<TRecord>
        where TRecord : MongoRecord
    {
        protected abstract string DatabaseName { get; }
        protected abstract string CollectionName { get; }

        private readonly IMongoClient _mongoClient;

        protected MongoRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
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
            => _mongoClient.GetDatabase(DatabaseName).GetCollection<TRecord>(CollectionName);
    }
}