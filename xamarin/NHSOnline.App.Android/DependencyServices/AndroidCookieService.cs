using System.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Android.Webkit;
using NHSOnline.App.Api.Client.Cookies;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidCookieService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    [SuppressMessage("Reliability", "CA2000", Justification = "Disposing is hard and causes crashes if we do it wrong")]
    public class AndroidCookieService : ICookieService
    {
        private static CookieManager CookieManager => CookieManager.Instance ??
                                                      throw new NotSupportedException(
                                                          "Could not get instance of Android CookieManager");

        public async Task SetCookie(ApiCookie apiCookie)
        {
            var callBack = new CallBackResult();

            CookieManager.SetCookie(apiCookie.Uri.ToString(), apiCookie.Value, callBack);

            await callBack.GetAwaitable().ResumeOnThreadPool();
        }

        public async Task ClearSessionCookies()
        {
            var callBack = new CallBackResult();

            CookieManager.RemoveSessionCookies(callBack);

            await callBack.GetAwaitable().ResumeOnThreadPool();
        }

        public Task<Cookie?> GetCookie(Uri uri, string cookieName)
        {
            var cookieHeader = CookieManager.GetCookie(uri.AbsoluteUri);
            var cookieCollection = ParseCookiesString(uri, cookieHeader);

            foreach (Cookie cookie in cookieCollection)
            {
                if (cookie.Name == cookieName)
                {
                    return Task.FromResult<Cookie?>(cookie);
                }
            }

            return Task.FromResult<Cookie?>(null);
        }

        private static CookieCollection ParseCookiesString(Uri uri, string? cookies)
        {
            if (cookies == null)
            {
                return new CookieCollection();
            }

            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(uri, cookies);
            return cookieContainer.GetCookies(uri);
        }
    }
}