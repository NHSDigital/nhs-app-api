using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> Post(string accessToken, HttpContext httpContext);
    }
}