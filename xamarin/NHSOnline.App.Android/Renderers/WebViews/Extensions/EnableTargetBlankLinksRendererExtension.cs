using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class EnableTargetBlankLinksRendererExtension : WebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;

        public EnableTargetBlankLinksRendererExtension(WebViewRenderer renderer)
            => _renderer = renderer;

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null)
            {
                _renderer.Control.Settings?.SetSupportMultipleWindows(false);
            }
        }
    }
}