using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    public interface IInfoRepository
    {
        Task<PostInfoResult> Create(UserAndInfo userAndInfo);
        Task<UserAndInfo> FindByNhsLoginId(string nhsLoginId);
        Task<IEnumerable<UserAndInfo>> FindByOdsCode(string odsCode);
        Task<IEnumerable<UserAndInfo>> FindByNhsNumber(string nhsNumber);
    }
}