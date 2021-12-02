using NHSOnline.App.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class UserAgentWebViewRendererExtension: WebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;

        public UserAgentWebViewRendererExtension(WebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null && _renderer.Control.Settings != null)
            {
                _renderer.Control.Settings.UserAgentString += $" {UserAgentService.Instance.NhsAppUserAgent}";
                BaseWebViewRenderer.UserAgent = _renderer.Control.Settings.UserAgentString;
            }
        }
    }
}