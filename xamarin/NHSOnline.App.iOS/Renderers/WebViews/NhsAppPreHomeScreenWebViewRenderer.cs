using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsAppPreHomeScreenWebview), typeof(NhsAppPreHomeScreenWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsAppPreHomeScreenWebViewRenderer : WkWebViewRenderer
    {
        private readonly JavascriptBridge<NhsAppPreHomeScreenWebview> _javascriptBridge;

        public NhsAppPreHomeScreenWebViewRenderer() : this(CustomConfiguration)
        {
        }

        private NhsAppPreHomeScreenWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _javascriptBridge = JavascriptBridge
                    .ForWebView(() => (NhsAppPreHomeScreenWebview)Element, "nativeApp")
                    .AddFunction("getNotificationsStatus", webView => webView.GetNotificationsStatus)
                    .AddFunction("requestPnsToken", webView => webView.RequestPnsToken)
                    .AddFunction("goToLoggedInHomeScreen", webView => webView.GoToLoggedInHomeScreen)
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
