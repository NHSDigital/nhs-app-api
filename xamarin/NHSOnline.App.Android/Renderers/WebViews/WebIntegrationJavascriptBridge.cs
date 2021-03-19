using Android.Webkit;
using Java.Interop;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;

using Object = Java.Lang.Object;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class WebIntegrationJavascriptBridge : Object
    {
        private readonly WebIntegrationWebView _webIntegrationWebView;

        public WebIntegrationJavascriptBridge(WebIntegrationWebView webIntegrationWebView)
        {
            _webIntegrationWebView = webIntegrationWebView;
        }

        [JavascriptInterface]
        [Export("goToPage")]
        public void GoToPage(string page)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _webIntegrationWebView.RedirectToNhsAppPage(page));
        }
    }
}
