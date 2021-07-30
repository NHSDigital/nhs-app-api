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
            foreach (var cookie in await cookieStore.GetAllCookiesAsync().ConfigureAwait(true))
            {
                if (cookie.IsSessionOnly)
                {
                    await cookieStore.DeleteCookieAsync(cookie).ConfigureAwait(true);
                }
            }
        }
    }
}