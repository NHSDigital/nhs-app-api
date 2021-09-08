using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews.Extensions
{
    internal class NavigationDelegateRendererExtension : IWebViewRendererExtension
    {
        private readonly WkWebViewRenderer _renderer;

        public NavigationDelegateRendererExtension(WkWebViewRenderer renderer)
        {
            _renderer = renderer;
        }
        public void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                _renderer.NavigationDelegate = new WebViewNavigationDelegate(_renderer);
            }
        }
    }
}