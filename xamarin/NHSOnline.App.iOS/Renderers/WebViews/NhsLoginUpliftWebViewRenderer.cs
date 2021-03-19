using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsLoginUpliftWebView), typeof(NhsLoginUpliftWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginUpliftWebViewRenderer : WkWebViewRenderer
    {
        private readonly JavascriptBridge<NhsLoginUpliftWebView> _javascriptBridge;

        public NhsLoginUpliftWebViewRenderer() : this(CustomConfiguration)
        {
        }

        private NhsLoginUpliftWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            this.InstallIProov();

            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsLoginUpliftWebView)Element, "nativeNhsLogin")
                .Apply(config.UserContentController);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new WebViewNavigationDelegate(this);
            }

            base.OnElementChanged(e);
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