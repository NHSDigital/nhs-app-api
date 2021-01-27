using System;
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
            var cookieManager = CookieManager.Instance ?? throw new InvalidOperationException("CookieManager.Instance was null");
            cookieManager.RemoveAllCookie();
        }
    }
}