using System.Threading.Tasks;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UserInfo.Repository
{
    public interface IInfoRepository
    {
        Task<RepositoryCreateResult<UserAndInfo>> Create(UserAndInfo userAndInfo);
        Task<RepositoryFindResult<UserAndInfo>> FindByNhsLoginId(string nhsLoginId);
        Task<RepositoryFindResult<UserAndInfo>> FindByOdsCode(string odsCode);
        Task<RepositoryFindResult<UserAndInfo>> FindByNhsNumber(string nhsNumber);
    }
}