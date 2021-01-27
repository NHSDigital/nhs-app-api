using System;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal abstract class WebViewLocatorStrategy
    {
        protected static TimeSpan WaitForWebContext { get; } = TimeSpan.FromSeconds(20);

        internal abstract void SwitchToWebView(WebViewContext webViewContext);

        internal abstract void ForEachWebView(Action<string> action);

        protected static bool IsWebViewContext(string context)
            => context.Contains("webview", StringComparison.OrdinalIgnoreCase);
    }
}
