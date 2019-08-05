using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi
{
    public interface IUserSessionManager
    {
        Task<bool> SignOutAsync(HttpContext httpContext);
    }
}