using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsLoginWebView), typeof(NhsLoginWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginWebViewRenderer : BaseWebViewRenderer
    {
        public NhsLoginWebViewRenderer() : base(CustomConfiguration)
        { }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}
