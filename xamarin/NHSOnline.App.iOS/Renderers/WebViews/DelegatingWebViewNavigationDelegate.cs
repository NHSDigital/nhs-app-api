using System;
using System.Runtime.CompilerServices;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Logging;
using WebKit;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    /// <summary>
    /// The default web view navigation delegate which does not handle the <see cref="WKNavigationDelegate.DidFailProvisionalNavigation"/> case.
    /// This wrapper converts that notification into a call of <see cref="WKNavigationDelegate.DidFailNavigation"/>
    /// </summary>
    internal sealed class DelegatingWebViewNavigationDelegate : WKNavigationDelegate
    {
        private readonly IWKNavigationDelegate _wrappedNavigationDelegate;

        public DelegatingWebViewNavigationDelegate(IWKNavigationDelegate wrappedNavigationDelegate)
        {
            _wrappedNavigationDelegate = wrappedNavigationDelegate;
        }

        private static ILogger Logger { get; } = NhsAppLogging.CreateLogger<DelegatingWebViewNavigationDelegate>();

        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                Log(navigationAction);

                if (IsNewWindow(navigationAction))
                {
                    // The default navigation delegate requests to load the page in the current webView
                    // and calls the Navigating event, resulting in the navigating event firing twice.
                    // We cancel the request for a new window and create a new request in the current webView.
                    decisionHandler(WKNavigationActionPolicy.Cancel);
                    webView.LoadRequest(navigationAction.Request);
                }
                else if (IsMainFrame(navigationAction))
                {
                    _wrappedNavigationDelegate.DecidePolicy(webView, navigationAction, decisionHandler);
                }
                else
                {
                    decisionHandler(WKNavigationActionPolicy.Allow);
                }
            });
        }

        private static bool IsNewWindow(WKNavigationAction navigationAction) => navigationAction.TargetFrame is null;
        private static bool IsMainFrame(WKNavigationAction navigationAction) => navigationAction.TargetFrame?.MainFrame ?? false;

        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                Log(LogLevel.Debug, navigation);
                _wrappedNavigationDelegate.DidStartProvisionalNavigation(webView, navigation);
            });
        }

        public override void DidReceiveServerRedirectForProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(() => Log(LogLevel.Debug, navigation));
        }

        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                LogError(navigation, error);

                // Call DidFailNavigation as default navigation handler does not handle DidFailProvisionalNavigation
                _wrappedNavigationDelegate.DidFailNavigation(webView, navigation, error);
            });
        }

        public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(() => Log(LogLevel.Debug, navigation));
        }

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                Log(LogLevel.Debug, navigation);
                _wrappedNavigationDelegate.DidFinishNavigation(webView, navigation);
            });
        }

        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                LogError(navigation, error);
                _wrappedNavigationDelegate.DidFailNavigation(webView, navigation, error);
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

        private static void Log(LogLevel level, WKNavigation navigation, [CallerMemberName] string method = "")
        {
            Logger.Log(
                level,
                "Navigation {Id:X} {Method}",
                navigation.ClassHandle.ToInt64(),
                method);
        }

        private static void LogError(WKNavigation navigation, NSError error, [CallerMemberName] string method = "")
        {
            Logger.LogError(
                "Navigation {Id:X} {Method}: {Description}",
                navigation.ClassHandle.ToInt64(),
                method,
                error.LocalizedDescription);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _wrappedNavigationDelegate.Dispose();
            }
        }
    }
}