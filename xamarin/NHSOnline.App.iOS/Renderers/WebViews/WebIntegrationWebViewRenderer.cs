using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews.Extensions;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebIntegrationWebView), typeof(WebIntegrationWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    [SuppressMessage("Reliability", "CA2000", Justification = "Disposing is hard and causes crashes if we do it wrong")]
    internal sealed class WebIntegrationWebViewRenderer : WkWebViewRenderer
    {
        private readonly JavascriptBridge<WebIntegrationWebView> _javascriptBridge;
        private readonly List<IWebViewRendererExtension> _extensions;

        public WebIntegrationWebViewRenderer() : this(CustomConfiguration)
        { }

        private WebIntegrationWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _extensions = new List<IWebViewRendererExtension>
            {
                new NavigationDelegateRendererExtension(this),
                new WebIntegrationRequestRendererExtension(this)
            };

            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (WebIntegrationWebView)Element, WebIntegrationWebView.JavascriptObjectName)
                .AddFunction("goToPage", webView => webView.GoToNhsAppPage)
                .AddFunction("addEventToCalendar", webView => webView.AddEventToCalendar)
                .AddFunction("startDownloadFromJson", webview => webview.StartDownload)
                .Apply(config.UserContentController);

            AllowsLinkPreview = false;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            foreach (var extension in _extensions)
            {
                extension.OnElementChanged(e);
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
