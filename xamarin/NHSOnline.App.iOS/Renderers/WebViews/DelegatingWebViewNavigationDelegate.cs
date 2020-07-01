using System;
using System.Runtime.CompilerServices;
using Foundation;
using Microsoft.Extensions.Logging;
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
            => _wrappedNavigationDelegate.DecidePolicy(webView, navigationAction, decisionHandler);

        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            Log(LogLevel.Debug, navigation);
            _wrappedNavigationDelegate.DidStartProvisionalNavigation(webView, navigation);
        }

        public override void DidReceiveServerRedirectForProvisionalNavigation(WKWebView webView, WKNavigation navigation)
            => Log(LogLevel.Debug, navigation);

        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            LogError(navigation, error);

            // Call DidFailNavigation as default navigation handler does not handle DidFailProvisionalNavigation
            _wrappedNavigationDelegate.DidFailNavigation(webView, navigation, error);
        }

        public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
            => Log(LogLevel.Debug, navigation);

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            Log(LogLevel.Debug, navigation);
            _wrappedNavigationDelegate.DidFinishNavigation(webView, navigation);
        }

        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            LogError(navigation, error);
            _wrappedNavigationDelegate.DidFailNavigation(webView, navigation, error);
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