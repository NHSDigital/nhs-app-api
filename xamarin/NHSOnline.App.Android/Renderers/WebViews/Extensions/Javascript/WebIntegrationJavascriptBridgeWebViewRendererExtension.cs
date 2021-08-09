using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript
{
    [SuppressMessage("Microsoft.Design", "CA1001", Justification = "If we manually dispose the bridge we get occasional crashes as Android still has a reference to it, see https://github.com/xamarin/xamarin-android/issues/1408#issuecomment-373236773")]
    internal sealed class WebIntegrationJavascriptBridgeWebViewRendererExtension : IWebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;
        private WebIntegrationJavascriptBridge? _webIntegrationJavascriptBridge;

        public WebIntegrationJavascriptBridgeWebViewRendererExtension(WebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement is WebIntegrationWebView)
            {
                _renderer.Control.RemoveJavascriptInterface(WebIntegrationWebView.JavascriptObjectName);
            }

            if (e.NewElement is WebIntegrationWebView newWebIntegrationWebView)
            {
                _webIntegrationJavascriptBridge = new WebIntegrationJavascriptBridge(newWebIntegrationWebView);
                _renderer.Control.AddJavascriptInterface(_webIntegrationJavascriptBridge, WebIntegrationWebView.JavascriptObjectName);
            }
        }
    }
}