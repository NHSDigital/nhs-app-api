using Android.Webkit;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidCookies))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public sealed class AndroidCookies : ICookies
    {
        public void Clear()
        {
            var cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
        }
    }
}