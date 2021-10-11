using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews.Extensions;
using WebKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(WebIntegrationWebView), typeof(WebIntegrationWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    [SuppressMessage("Reliability", "CA2000", Justification = "Disposing is hard and causes crashes if we do it wrong")]
    internal sealed class WebIntegrationWebViewRenderer : BaseWebViewRenderer
    {
        private readonly JavascriptBridge<WebIntegrationWebView> _javascriptBridge;

        public WebIntegrationWebViewRenderer() : this(CustomConfiguration)
        { }

        private WebIntegrationWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (WebIntegrationWebView)Element, WebIntegrationWebView.JavascriptObjectName)
                .AddFunction("goToPage", webView => webView.GoToNhsAppPage)
                .AddFunction("addEventToCalendar", webView => webView.AddEventToCalendar)
                .AddFunction("startDownloadFromJson", webview => webview.StartDownload)
                .Apply(config.UserContentController);

            AddExtension(new WebIntegrationRequestRendererExtension(this));
            AddExtension(new PageLoadRedirectAggregatorExtension(this));
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
