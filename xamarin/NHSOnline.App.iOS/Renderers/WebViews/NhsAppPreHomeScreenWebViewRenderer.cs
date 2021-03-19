using System.Net;
using System.Threading.Tasks;
using Foundation;
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
                    .AddFunction("goToLoggedInHomeScreen", webView => webView.GoToLoggedInHomeScreen)
                    .Apply(config.UserContentController);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new WebViewNavigationDelegate(this);
            }

            if (e.OldElement is NhsAppPreHomeScreenWebview oldNhsAppPreHomeWebview)
            {
                oldNhsAppPreHomeWebview.SetCookie = null;
            }

            if (e.NewElement is NhsAppPreHomeScreenWebview newNhsAppWebPreHomeView)
            {
                newNhsAppWebPreHomeView.SetCookie = SetCookie;
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

        private async Task SetCookie(Cookie cookie)
        {
            using var nsHttpCookie = new NSHttpCookie(cookie);

            await Configuration.WebsiteDataStore.HttpCookieStore.SetCookieAsync(nsHttpCookie).ConfigureAwait(true);
        }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}
