using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public interface IInfoService
    {
        Task<PostInfoResult> Send(AccessToken accessToken, InfoUserProfile userProfile);
        Task<GetInfoResult> GetInfo(AccessToken accessToken);
        Task<GetInfoResult> GetInfoByNhsNumber(string nhsNumber);
        Task<GetInfoResult> GetInfoByOdsCode(string odsCode);
    }
}