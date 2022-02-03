using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Foundation;
using NHSOnline.App.Api.Client.Cookies;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Threading;
using WebKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosCookieService))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosCookieService : ICookieService
    {
        public async Task SetCookie(ApiCookie apiCookie)
        {
            using var headerDictionary = new NSDictionary("Set-Cookie", apiCookie.Value);

            var cookies = NSHttpCookie.CookiesWithResponseHeaderFields(headerDictionary, apiCookie.Uri);

            var cookieStore = WKWebsiteDataStore.DefaultDataStore.HttpCookieStore;
            foreach (var nsHttpCookie in cookies)
            {
               await cookieStore.SetCookieAsync(nsHttpCookie).ResumeOnThreadPool();
            }
        }

        public async Task ClearSessionCookies()
        {
            var cookieStore = WKWebsiteDataStore.DefaultDataStore.HttpCookieStore;
            foreach (var cookie in await cookieStore.GetAllCookiesAsync().PreserveThreadContext())
            {
                if (cookie.IsSessionOnly)
                {
                    await cookieStore.DeleteCookieAsync(cookie).PreserveThreadContext();
                }
            }

            var nsHttpCookieStore = NSHttpCookieStorage.SharedStorage.Cookies.ToList();
            foreach (var cookie in nsHttpCookieStore)
            {
                if (cookie.IsSessionOnly)
                {
                    NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);
                }
            }
        }

        public async Task<Cookie?> GetCookie(Uri uri, string cookieName)
        {
            var cookieStore = WKWebsiteDataStore.DefaultDataStore.HttpCookieStore;
            var cookies = await cookieStore.GetAllCookiesAsync().ResumeOnThreadPool() ??
                          Array.Empty<NSHttpCookie>();

            foreach (NSHttpCookie cookie in cookies)
            {
                if (cookie.Name == cookieName &&
                    uri.Host.Contains(cookie.Domain, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
                }
            }

            return null;
        }
    }
}