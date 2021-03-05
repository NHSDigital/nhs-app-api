using System;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript
{
    internal sealed class NhsAppJavascriptBridgeWebViewRendererExtension : IWebViewRendererExtension, IDisposable
    {
        private readonly WebViewRenderer _renderer;
        private NhsAppJavascriptBridge? _nhsAppJavascriptBridge;

        public NhsAppJavascriptBridgeWebViewRendererExtension(WebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement is NhsAppWebView)
            {
                _renderer.Control.RemoveJavascriptInterface(NhsAppJavascriptBridge.JavascriptObjectName);
                _nhsAppJavascriptBridge?.Dispose();
            }

            if (e.NewElement is NhsAppWebView newNhsAppWebView)
            {
                _nhsAppJavascriptBridge = new NhsAppJavascriptBridge(newNhsAppWebView);
                _renderer.Control.AddJavascriptInterface(_nhsAppJavascriptBridge, NhsAppJavascriptBridge.JavascriptObjectName);
            }
        }

        public void Dispose()
        {
            _nhsAppJavascriptBridge?.Dispose();
        }
    }
}