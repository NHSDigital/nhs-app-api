using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Foundation;
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
        private const string JavaScriptFunction = @"window.nativeApp = {};
                                                    window.nativeApp.openWebIntegration = function(request) {
                                                        window.webkit.messageHandlers.openWebIntegration.postMessage(request);
                                                    }";

        private readonly NSString _javascriptFunction;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public NhsAppWebViewRenderer() : this(CustomConfiguration)
        {
        }

        private NhsAppWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            var userController = config.UserContentController;
            _javascriptFunction = new NSString(JavaScriptFunction);
            var script = new WKUserScript(_javascriptFunction, WKUserScriptInjectionTime.AtDocumentStart, false);
            userController.AddUserScript(script);
            _disposables.Add(script);
            var handler = ScriptMessageHandler.For(() => ((NhsAppWebView)Element).OpenWebIntegrationCommand);
            userController.AddScriptMessageHandler(handler, "openWebIntegration");
            _disposables.Add(handler);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new DelegatingWebViewNavigationDelegate(NavigationDelegate);
            }

            if (e.OldElement is NhsAppWebView oldNhsAppWebView)
            {
                oldNhsAppWebView.SetCookie = null;
            }

            if (e.NewElement is NhsAppWebView newNhsAppWebView)
            {
                newNhsAppWebView.SetCookie = SetCookie;
            }

            base.OnElementChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables.ForEach(d => d.Dispose());
                _javascriptFunction.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task SetCookie(Cookie cookie)
        {
            using var nsHttpCookie = new NSHttpCookie(cookie);

            await Configuration.WebsiteDataStore.HttpCookieStore.SetCookieAsync(nsHttpCookie).ConfigureAwait(true);
        }

        private static WKWebViewConfiguration CustomConfiguration => new WKWebViewConfiguration
        {
            ApplicationNameForUserAgent = " nhsapp-ios/1.0.0",
            SuppressesIncrementalRendering = true
        };
    }
}
