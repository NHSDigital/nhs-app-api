using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews.Extensions
{
    internal class UIDelegateRendererExtension : IWebViewRendererExtension
    {
        private readonly WkWebViewRenderer _renderer;

        public UIDelegateRendererExtension(WkWebViewRenderer renderer)
        {
            _renderer = renderer;
        }
        public void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                _renderer.UIDelegate = new WebViewUIDelegate();
            }
        }
    }
}