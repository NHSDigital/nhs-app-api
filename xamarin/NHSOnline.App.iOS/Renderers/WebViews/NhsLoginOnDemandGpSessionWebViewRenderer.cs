using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsLoginOnDemandGpSessionWebView), typeof(NhsLoginOnDemandGpSessionWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginOnDemandGpSessionWebViewRenderer : WkWebViewRenderer
    {
        public NhsLoginOnDemandGpSessionWebViewRenderer() : this(CustomConfiguration)
        {
        }

        private NhsLoginOnDemandGpSessionWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {

        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new WebViewNavigationDelegate(this);
            }

            base.OnElementChanged(e);
        }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}
