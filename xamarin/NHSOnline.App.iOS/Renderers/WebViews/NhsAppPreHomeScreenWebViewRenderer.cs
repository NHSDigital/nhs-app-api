using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews.Extensions;
using WebKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsAppPreHomeScreenWebview), typeof(NhsAppPreHomeScreenWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsAppPreHomeScreenWebViewRenderer : BaseWebViewRenderer
    {
        private readonly JavascriptBridge<NhsAppPreHomeScreenWebview> _javascriptBridge;

        public NhsAppPreHomeScreenWebViewRenderer() : this(CustomConfiguration)
        { }

        private NhsAppPreHomeScreenWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsAppPreHomeScreenWebview)Element, "nativeApp")
                .AddFunction("getNotificationsStatus", webView => webView.GetNotificationsStatus)
                .AddFunction("fetchBiometricStatus", webView => webView.FetchBiometricStatus)
                .AddFunction("updateBiometricRegistrationWithToken", webView => webView.UpdateBiometricRegistration)
                .AddFunction("requestPnsToken", webView => webView.RequestPnsToken)
                .AddFunction("goToLoggedInHomeScreen", webView => webView.GoToLoggedInHomeScreen)
                .AddFunction("onSessionExpiring", webView => webView.OnSessionExpiring)
                .AddFunction("sessionExpired", webView => webView.SessionExpired)
                .AddFunction("logout", webView => webView.Logout)
                .AddFunction("requestNotificationsRegistration", webView => webView.RequestNotificationsRegistration)
                .AddFunction("sendNotificationsRegistration", webView => webView.SetNotificationsRegistration)
                .Apply(config.UserContentController);

            AddExtension(new PageNavigationAggregatorExtension(this));
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
