using System;
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

        private IMongoCollection<TRecord> GetCollection()
            => _mongoClient.GetDatabase(DatabaseName).GetCollection<TRecord>(CollectionName);
    }
}