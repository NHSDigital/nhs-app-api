using System.Threading.Tasks;
using NHSOnline.App.Api.Client.Cookies;

namespace NHSOnline.App.DependencyServices
{
    public interface ICookieService
    {
        Task SetCookie(ApiCookie apiCookie);
        Task ClearSessionCookies();
    }
}