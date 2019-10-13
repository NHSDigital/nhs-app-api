using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> Post(string accessToken);
    }
}