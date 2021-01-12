using System;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal sealed class MongoIm1Cache: IMongoIm1Cache
    {
        private readonly IRepository<Im1CacheRecord> _repository;
        
        public MongoIm1Cache(IRepository<Im1CacheRecord> repository)
        {
            _repository = repository;
        }

        public async Task Save(string key, string token)
        {
            var record = new Im1CacheRecord
            {
                Key = key,
                Token = token
            };

            await _repository.Create(record, nameof(Im1CacheRecord));
        }

        public async Task<Option<string>> Get(string key)
        {
            var result = await _repository.Find(r => r.Key == key, nameof(Im1CacheRecord), 1);

            return result.Accept(new FindResultVisitor());
        }

        public async Task<bool> Delete(string key)
        {
            var result = await _repository.Delete(r => r.Key == key, nameof(Im1CacheRecord));

            return result.Accept(new DeleteResultVisitor());
        }

        private sealed class FindResultVisitor : IRepositoryFindResultVisitor<Im1CacheRecord, Option<string>>
        {
            public Option<string> Visit(RepositoryFindResult<Im1CacheRecord>.NotFound result)
                => Option.None<string>();

            public Option<string> Visit(RepositoryFindResult<Im1CacheRecord>.Found result)
            {
                if (result.Records.Count > 0)
                {
                    return Option.Some(result.Records.First().Token);
                }

                return Option.None<string>();
            }

            public Option<string> Visit(RepositoryFindResult<Im1CacheRecord>.RepositoryError result)
                => throw new InvalidOperationException("Repository error reading from Im1Cache");
        }

        private sealed class DeleteResultVisitor : IRepositoryDeleteResultVisitor<Im1CacheRecord, bool>
        {
            public bool Visit(RepositoryDeleteResult<Im1CacheRecord>.Deleted result) => true;

            public bool Visit(RepositoryDeleteResult<Im1CacheRecord>.NotFound result) => false;

            public bool Visit(RepositoryDeleteResult<Im1CacheRecord>.RepositoryError result)
                => throw new InvalidOperationException("Repository error deleting from Im1Cache");
        }
    }
}