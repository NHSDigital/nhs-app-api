using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class UserAgentWebViewRendererExtension: IWebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;

        public UserAgentWebViewRendererExtension(WebViewRenderer renderer)
            => _renderer = renderer;

        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null && _renderer.Control.Settings != null)
            {
                _renderer.Control.Settings.UserAgentString += " nhsapp-android/1.0.0";
            }
        }
    }
}