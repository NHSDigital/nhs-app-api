using System;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript
{
    internal sealed class WebIntegrationJavascriptBridgeWebViewRendererExtension : IWebViewRendererExtension, IDisposable
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
                _webIntegrationJavascriptBridge?.Dispose();
            }

            if (e.NewElement is WebIntegrationWebView newWebIntegrationWebView)
            {
                _webIntegrationJavascriptBridge = new WebIntegrationJavascriptBridge(newWebIntegrationWebView);
                _renderer.Control.AddJavascriptInterface(_webIntegrationJavascriptBridge, WebIntegrationWebView.JavascriptObjectName);
            }
        }

        public void Dispose()
        {
            _webIntegrationJavascriptBridge?.Dispose();
        }
    }
}