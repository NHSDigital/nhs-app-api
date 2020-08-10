using System;
using System.Runtime.CompilerServices;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Logging;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class WebViewNavigationDelegate : WKNavigationDelegate
    {
        private readonly WkWebViewRenderer _renderer;

        public WebViewNavigationDelegate(WkWebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        private WebView WebView => (WebView)_renderer.Element;

        private static ILogger Logger { get; } = NhsAppLogging.CreateLogger<WebViewNavigationDelegate>();

        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                Log(navigationAction);

                if (IsNewWindow(navigationAction))
                {
                    // Load requests for a new window in the main window
                    decisionHandler(WKNavigationActionPolicy.Cancel);
                    webView.LoadRequest(navigationAction.Request);
                }
                else if (IsMainFrame(navigationAction))
                {
                    var navEvent = WebNavigationEvent.NewPage;
                    var request = navigationAction.Request;
                    var lastUrl = request.Url.ToString();
                    var args = new WebNavigatingEventArgs(navEvent, new UrlWebViewSource { Url = lastUrl }, lastUrl);

                    WebView.SendNavigating(args);

                    var decision = args.Cancel switch
                    {
                        true => WKNavigationActionPolicy.Cancel,
                        false => WKNavigationActionPolicy.Allow
                    };
                    decisionHandler(decision);
                }
                else
                {
                    decisionHandler(WKNavigationActionPolicy.Allow);
                }
            });
        }

        private static bool IsNewWindow(WKNavigationAction navigationAction) => navigationAction.TargetFrame is null;
        private static bool IsMainFrame(WKNavigationAction navigationAction) => navigationAction.TargetFrame?.MainFrame ?? false;

        public override void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                Log(navigationResponse);

                decisionHandler(WKNavigationResponsePolicy.Allow);
            });
        }

        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(action: () => Log(navigation));
        }

        public override void DidReceiveServerRedirectForProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(() => Log(navigation));
        }

        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                LogError(navigation, error);

                var url = _renderer?.Url?.AbsoluteUrl?.ToString();
                var webViewSource = new UrlWebViewSource { Url = url };
                var eventArgs = new WebNavigatedEventArgs(WebNavigationEvent.NewPage, webViewSource, url, WebNavigationResult.Failure);

                WebView.SendNavigated(eventArgs);
            });
        }

        public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(() => Log(navigation));
        }

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                Log(navigation);

                var url = _renderer?.Url?.AbsoluteUrl?.ToString();
                var args = new WebNavigatedEventArgs(WebNavigationEvent.NewPage, WebView.Source, url, WebNavigationResult.Success);
                WebView.SendNavigated(args);
            });
        }

        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                LogError(navigation, error);

                var url = _renderer?.Url?.AbsoluteUrl?.ToString();
                var args = new WebNavigatedEventArgs(WebNavigationEvent.NewPage, WebView.Source, url, WebNavigationResult.Success);
                WebView.SendNavigated(args);
            });
        }

        private static void Log(WKNavigationAction navigationAction, [CallerMemberName] string method = "")
        {
            Logger.LogDebug(
                "NavigationAction {Type} {Target} {Method}",
                navigationAction.NavigationType,
                navigationAction.TargetFrame switch
                {
                    { MainFrame: true} => "Main Frame",
                    { MainFrame: false } => "Sub Frame",
                    null => "New Window"
                },
                method);
        }

        private static void Log(WKNavigationResponse navigationResponse, [CallerMemberName] string method = "")
        {
            Logger.LogDebug(
                "NavigationResponse {Response} {Target} {Method}",
                navigationResponse.Response switch
                {
                    NSHttpUrlResponse httpResponse => $"{httpResponse.Url} {httpResponse.StatusCode}",
                    {} urlResponse => urlResponse.Url,
                    null => "{null}"
                },
                navigationResponse.IsForMainFrame switch
                {
                    true => "Main Frame",
                    false => "Sub Frame"
                },
                method);
        }

        private static void Log(WKNavigation navigation, [CallerMemberName] string method = "")
        {
            Logger.LogDebug(
                "Navigation {Id:X} {Method}",
                navigation.ClassHandle.ToInt64(),
                method);
        }

        private static void LogError(WKNavigation navigation, NSError error, [CallerMemberName] string method = "")
        {
            Logger.LogDebug(
                "Navigation {Id:X} {Method}: {Description}",
                navigation.ClassHandle.ToInt64(),
                method,
                error.LocalizedDescription);
        }
    }
}