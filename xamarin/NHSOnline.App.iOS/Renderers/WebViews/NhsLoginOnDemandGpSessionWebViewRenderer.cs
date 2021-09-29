using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsLoginOnDemandGpSessionWebView), typeof(NhsLoginOnDemandGpSessionWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginOnDemandGpSessionWebViewRenderer : BaseWebViewRenderer
    {
        public NhsLoginOnDemandGpSessionWebViewRenderer() : this(CustomConfiguration)
        { }

        private NhsLoginOnDemandGpSessionWebViewRenderer(WKWebViewConfiguration config) : base(config)
        { }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}
