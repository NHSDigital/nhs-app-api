using Foundation;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosCookies))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosCookies : ICookies
    {
        public void Clear()
        {
            var cookieStorage = NSHttpCookieStorage.SharedStorage;
            foreach (var cookie in cookieStorage.Cookies)
            {
                cookieStorage.DeleteCookie(cookie);
            }
        }
    }
}