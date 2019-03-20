using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support
{
    public interface IUserSessionManager
    {
        Task<bool> SignOutAsync(HttpContext httpContext);
    }
}