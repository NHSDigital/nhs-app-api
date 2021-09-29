using Foundation;
using WebKit;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class WebViewUIDelegate : WKUIDelegate
    {
        public override WKWebView CreateWebView(WKWebView webView, WKWebViewConfiguration configuration, WKNavigationAction navigationAction, WKWindowFeatures windowFeatures)
        {
            if (navigationAction.Request.Url != null)
            {
                using var newRequest = new NSUrlRequest(navigationAction.Request.Url);
                webView.LoadRequest(newRequest);
            }

            #pragma warning disable 8603 // cant make return type WKWebView? as it wont match override and cant return webView as it causes fatal error as it hasn't been created yet
                return null;
            #pragma warning restore 8603
        }
    }
}