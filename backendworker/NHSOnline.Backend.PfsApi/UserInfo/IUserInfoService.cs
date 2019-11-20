using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public interface IUserInfoService
    {
        Task<UserInfoResult> Update(string accessToken, HttpContext httpContext);
    }
}