using NHSOnline.Backend.Repository.SqlApi;

namespace NHSOnline.Backend.UserInfo.Repository
{
    public interface IUserAndInfoSqlApiRepositoryFactory
    {
        public ISqlApiRepository<UserAndInfo> GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey partitionKey);
    }
}