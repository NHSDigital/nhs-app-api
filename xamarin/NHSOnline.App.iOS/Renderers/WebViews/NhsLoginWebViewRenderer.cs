using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsLoginWebView), typeof(NhsLoginWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginWebViewRenderer : WkWebViewRenderer
    {
        public NhsLoginWebViewRenderer() : base(CustomConfiguration)
        {}

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new DelegatingWebViewNavigationDelegate(NavigationDelegate);
            }

            base.OnElementChanged(e);
        }

        private static WKWebViewConfiguration CustomConfiguration => new WKWebViewConfiguration
        {
            ApplicationNameForUserAgent = " nhsapp-ios/1.0.0",
            SuppressesIncrementalRendering = true
        };
    }
}