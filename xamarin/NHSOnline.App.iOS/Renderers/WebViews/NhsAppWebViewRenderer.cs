using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsAppWebView), typeof(NhsAppWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsAppWebViewRenderer : BaseWebViewRenderer
    {
        private readonly JavascriptBridge<NhsAppWebView> _javascriptBridge;

        public NhsAppWebViewRenderer() : this(CustomConfiguration)
        { }

        private NhsAppWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsAppWebView) Element, "nativeApp")
                .AddFunction("openWebIntegration", webView => webView.OpenWebIntegration)
                .AddFunction("openPostWebIntegration", webView => webView.OpenPostWebIntegration)
                .AddFunction("startNhsLoginUplift", webView => webView.StartNhsLoginUplift)
                .AddFunction("getNotificationsStatus", webView => webView.GetNotificationsStatus)
                .AddFunction("requestPnsToken", webView => webView.RequestPnsToken)
                .AddFunction("fetchBiometricStatus", webView => webView.FetchBiometricStatus)
                .AddFunction("updateBiometricRegistrationWithToken", webView => webView.UpdateBiometricRegistration)
                .AddFunction("openBrowserOverlay", webView => webView.OpenBrowserOverlay)
                .AddFunction("openAppSettings", webView => webView.OpenSettings)
                .AddFunction("setMenuBarItem", webView => webView.SetMenuBarItem)
                .AddFunction("clearMenuBarItem", webView => webView.ClearMenuBarItem)
                .AddFunction("startDownloadFromJson", webview => webview.StartDownload)
                .AddFunction("addEventToCalendar", webView => webView.AddEventToCalendar)
                .AddFunction("displayPageLeaveWarning", webView => webView.DisplayPageLeaveWarning)
                .AddFunction("displayKeywordReplyPageLeaveWarning", webView => webView.DisplayKeywordReplyPageLeaveWarning)
                .AddFunction("onSessionExpiring", webView => webView.OnSessionExpiring)
                .AddFunction("createOnDemandGpSession", webView => webView.CreateOnDemandGpSession)
                .AddFunction("logout", webView => webView.Logout)
                .AddFunction("sessionExpired", webView => webView.SessionExpired)
                .AddFunction("fetchNativeAppVersion", webView => webView.FetchNativeAppVersion)
                .AddFunction("setBadgeCount", webView => webView.SetBadgeCount)
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
