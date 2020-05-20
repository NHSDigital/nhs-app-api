using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Repository
{
    public interface IRepository<TRecord>
    {
        public Task<RepositoryCreateResult<TRecord>> Create(
            TRecord record,
            string recordName);

        public Task<RepositoryCreateResult<TRecord>> CreateOrUpdate(
            Expression<Func<TRecord, bool>> filter,
            TRecord record,
            string recordName);

        public Task<RepositoryFindResult<TRecord>> Find(
            Expression<Func<TRecord, bool>> filter,
            string recordName);

        public Task<RepositoryDeleteResult<TRecord>> Delete(
            Expression<Func<TRecord, bool>> filter,
            string recordName);
    }
}