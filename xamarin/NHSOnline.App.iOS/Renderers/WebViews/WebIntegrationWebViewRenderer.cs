using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebIntegrationWebView), typeof(WebIntegrationWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class WebIntegrationWebViewRenderer : WkWebViewRenderer
    {
        private readonly JavascriptBridge<WebIntegrationWebView> _javascriptBridge;

        public WebIntegrationWebViewRenderer() : this(CustomConfiguration)
        { }

        private WebIntegrationWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (WebIntegrationWebView)Element, WebIntegrationWebView.JavascriptObjectName)
                .AddFunction("goToPage", webView => webView.RedirectToNhsAppPage)
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

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _javascriptBridge.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
