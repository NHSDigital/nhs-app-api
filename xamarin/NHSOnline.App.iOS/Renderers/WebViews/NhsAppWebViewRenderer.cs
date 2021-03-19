using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsAppWebView), typeof(NhsAppWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsAppWebViewRenderer : WkWebViewRenderer
    {
        private readonly JavascriptBridge<NhsAppWebView> _javascriptBridge;

        public NhsAppWebViewRenderer() : this(CustomConfiguration)
        {
        }

        private NhsAppWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsAppWebView) Element, "nativeApp")
                .AddFunction("openWebIntegration", webView => webView.OpenWebIntegration)
                .AddFunction("startNhsLoginUplift", webView => webView.StartNhsLoginUplift)
                .AddFunction("getNotificationsStatus", webView => webView.GetNotificationsStatus)
                .AddFunction("fetchBiometricSpec", webView => webView.FetchBiometricSpec)
                .AddFunction("updateBiometricRegistrationWithToken", webView => webView.UpdateBiometricRegistration)
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
