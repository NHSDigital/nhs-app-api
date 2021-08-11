using System;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.App.Api.Client.Cookies;

namespace NHSOnline.App.DependencyServices
{
    public interface ICookieService
    {
        Task SetCookie(ApiCookie apiCookie);
        Task ClearSessionCookies();
        Task<Cookie?> GetCookie(Uri uri, string cookieName);
    }
}