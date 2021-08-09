using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript
{
    [SuppressMessage("Microsoft.Design", "CA1001", Justification = "If we manually dispose the bridge we get occasional crashes as Android still has a reference to it, see https://github.com/xamarin/xamarin-android/issues/1408#issuecomment-373236773")]
    internal sealed class PreHomeScreenJavascriptBridgeWebViewRendererExtension : IWebViewRendererExtension
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
            }

            if (e.NewElement is NhsAppPreHomeScreenWebview newNhsAppPreHomeScreenWebView)
            {
                _nhsAppPreHomeJavascriptBridge = new NhsAppPreHomeJavascriptBridge(newNhsAppPreHomeScreenWebView);
                _renderer.Control.AddJavascriptInterface(_nhsAppPreHomeJavascriptBridge, NhsAppPreHomeJavascriptBridge.JavascriptObjectName);
            }
        }
    }
}