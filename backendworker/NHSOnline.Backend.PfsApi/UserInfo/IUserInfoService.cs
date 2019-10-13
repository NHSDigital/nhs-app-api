using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public interface IUserInfoService
    {
        Task<UserInfoResult> Update(string accessToken);
    }
}