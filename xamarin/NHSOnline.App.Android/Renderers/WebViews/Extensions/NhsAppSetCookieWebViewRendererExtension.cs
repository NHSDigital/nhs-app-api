using System.Net;
using System.Threading.Tasks;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class NhsAppSetCookieWebViewRendererExtension: IWebViewRendererExtension
    {
        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement is NhsAppWebView oldNhsAppWebView)
            {
                oldNhsAppWebView.SetCookie = null;
            }

            if (e.NewElement is NhsAppWebView newNhsAppWebView)
            {
                newNhsAppWebView.SetCookie = SetCookie;
            }
        }

        private static Task SetCookie(Cookie cookie)
        {
            CookieManager.Instance.SetCookie(cookie.Domain, cookie.ToString());
            return Task.CompletedTask;
        }
    }
}