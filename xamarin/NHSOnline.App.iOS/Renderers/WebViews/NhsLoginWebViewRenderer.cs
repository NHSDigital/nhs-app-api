using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsLoginWebView), typeof(NhsLoginWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginWebViewRenderer : BaseWebViewRenderer
    {
        private readonly JavascriptBridge<NhsLoginWebView> _javascriptBridge;

        public NhsLoginWebViewRenderer() : this(CustomConfiguration)
        { }

        private NhsLoginWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            this.InstallIProov();

            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsLoginWebView)Element, "nativeNhsLogin")
                .Apply(config.UserContentController);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _javascriptBridge.Dispose();
            }

            base.Dispose(disposing);
        }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}
