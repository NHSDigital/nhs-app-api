using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal abstract class WebViewLocatorStrategy
    {
        protected static TimeSpan WaitForWebContext { get; } = TimeSpan.FromSeconds(10);

        internal static WebViewLocatorStrategy MultipleContexts(IContextAware driver)
            => new MultipleContextsWebViewLocatorStrategy(driver);

        internal static WebViewLocatorStrategy MultipleWindows<TDriver>(TDriver driver)
            where TDriver : class, IWebDriver, IContextAware
            => new MultipleWindowsWebViewLocatorStrategy<TDriver>(driver);

        internal abstract void SwitchToWebView(WebViewContext webViewContext);

        internal abstract void ForEachWebView(Action<string> action);

        protected static bool IsWebViewContext(string context)
            => context.Contains("webview", StringComparison.OrdinalIgnoreCase);
    }
}