using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NHSOnline.App.Areas.Cookies
{
    internal class CookieHandler : ICookieHandler
    {
        public async Task AddCookies(ICookieView view, Uri cookieDomain,  CookieContainer cookies)
        {
            foreach (var cookie in cookies.GetCookies(cookieDomain).Cast<Cookie>())
            {
                await view.AddCookie(cookie).PreserveThreadContext();
            }
        }
    }
}