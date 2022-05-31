using System.Threading.Tasks;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UserInfo.Repository
{
    public interface IInfoRepository
    {
        Task<RepositoryCreateResult<UserAndInfo>> CreateOrUpdatePrimary(UserAndInfo userAndInfo);
        Task<RepositoryCreateResult<UserAndInfo>> CreateOrUpdateNhsNumberRecord(UserAndInfo userAndInfo);
        Task<RepositoryCreateResult<UserAndInfo>> CreateOrUpdateOdsCodeRecord(UserAndInfo userAndInfo);
        Task<RepositoryDeleteResult<UserAndInfo>> DeleteNhsNumberRecord(string nhsNumber, string nhsLoginId);
        Task<RepositoryDeleteResult<UserAndInfo>> DeleteOdsCodeRecord(string odsCode, string nhsLoginId);
        Task<RepositoryFindResult<UserAndInfo>> FindByNhsLoginId(string nhsLoginId);
        Task<RepositoryFindResult<UserAndInfo>> FindByOdsCode(string odsCode);
        Task<RepositoryFindResult<UserAndInfo>> FindByNhsNumber(string nhsNumber);
    }
}