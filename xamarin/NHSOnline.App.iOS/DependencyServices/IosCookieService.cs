using System.Net;
using System.Threading.Tasks;
using Foundation;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Threading;
using WebKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosCookieService))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosCookieService : ICookieService
    {
        public async Task SetCookie(Cookie cookie)
        {
            var cookieStorage = WKWebsiteDataStore.DefaultDataStore.HttpCookieStore;
            using var nsHttpCookie = new NSHttpCookie(cookie);

            var cookieSetCompletionSource = new TaskCompletionSource<object?>();
            void CompletionHandler()
            {
                cookieSetCompletionSource.SetResult(null);
            }

            cookieStorage.SetCookie(nsHttpCookie, CompletionHandler);
            await cookieSetCompletionSource.Task.ResumeOnThreadPool();
        }
    }
}