using System.Threading.Tasks;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public interface ISqlApiRepository<TRecord>
    {
        public Task<RepositoryCreateResult<TRecord>> CreateOrUpdate(TRecord record, string recordName,
            string partitionKeyValue);
        public Task<RepositoryDeleteResult<TRecord>> Delete(string id, string partitionKeyValue, string recordName);
        public Task<RepositoryFindResult<TRecord>> Find(string id, string partitionKeyValue, string recordName);
        public Task<RepositoryFindResult<TRecord>> Find(Expression<Func<TRecord, bool>> filter, string partitionKeyValue, string recordName);
        public Task<RepositoryFindResult<TRecord>> Find(Func<IQueryable<TRecord>, IQueryable<TRecord>> query, string recordName);
    }
}