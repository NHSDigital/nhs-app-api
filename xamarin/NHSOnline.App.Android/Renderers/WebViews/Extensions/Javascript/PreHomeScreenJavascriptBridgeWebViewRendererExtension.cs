using System;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript
{
    internal sealed class PreHomeScreenJavascriptBridgeWebViewRendererExtension : IWebViewRendererExtension, IDisposable
    {
        private readonly WebViewRenderer _renderer;
        private NhsAppPreHomeJavascriptBridge? _nhsAppPreHomeJavascriptBridge;

        public PreHomeScreenJavascriptBridgeWebViewRendererExtension(WebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement is NhsAppPreHomeScreenWebview)
            {
                _renderer.Control.RemoveJavascriptInterface(NhsAppPreHomeJavascriptBridge.JavascriptObjectName);
                _nhsAppPreHomeJavascriptBridge?.Dispose();
            }

            if (e.NewElement is NhsAppPreHomeScreenWebview newNhsAppPreHomeScreenWebView)
            {
                _nhsAppPreHomeJavascriptBridge = new NhsAppPreHomeJavascriptBridge(newNhsAppPreHomeScreenWebView);
                _renderer.Control.AddJavascriptInterface(_nhsAppPreHomeJavascriptBridge, NhsAppPreHomeJavascriptBridge.JavascriptObjectName);
            }
        }

        public void Dispose()
        {
            _nhsAppPreHomeJavascriptBridge?.Dispose();
        }
    }
}