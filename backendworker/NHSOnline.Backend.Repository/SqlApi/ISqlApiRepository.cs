using System.Threading.Tasks;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public interface ISqlApiRepository<TRecord>
    {
        public Task<RepositoryCreateResult<TRecord>> CreateOrUpdate(TRecord record, string recordName, string partitionKeyValue);
        public Task<RepositoryDeleteResult<TRecord>> Delete(string id, string partitionKeyValue, string recordName);
    }
}