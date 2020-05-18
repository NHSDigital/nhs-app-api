using System.Threading.Tasks;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    public interface IInfoRepository
    {
        Task<RepositoryCreateResult<UserAndInfo>> Create(UserAndInfo userAndInfo);
        Task<RepositoryFindResult<UserAndInfo>> FindByNhsLoginId(string nhsLoginId);
        Task<RepositoryFindResult<UserAndInfo>> FindByOdsCode(string odsCode);
        Task<RepositoryFindResult<UserAndInfo>> FindByNhsNumber(string nhsNumber);
    }
}