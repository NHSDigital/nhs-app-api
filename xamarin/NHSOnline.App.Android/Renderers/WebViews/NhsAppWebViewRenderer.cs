using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(NhsAppWebView), typeof(NhsAppWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsAppWebViewRenderer: WebViewRenderer
    {
        public NhsAppWebViewRenderer(Context context) : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Settings.UserAgentString += " nhsapp-android/1.0.0";
            }

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