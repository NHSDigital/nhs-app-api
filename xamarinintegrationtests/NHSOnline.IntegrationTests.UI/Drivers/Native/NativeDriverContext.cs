using System;
using System.Linq;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class NativeDriverContext
    {
        private readonly IContextAware _contextAwareDriver;
        private readonly WebViewLocatorStrategy _webViewLocatorStrategy;
        private readonly string _nativeContextName;

        internal NativeDriverContext(IContextAware contextAwareDriver, WebViewLocatorStrategy webViewLocatorStrategy)
        {
            _contextAwareDriver = contextAwareDriver;
            _webViewLocatorStrategy = webViewLocatorStrategy;

            var contexts = contextAwareDriver.Contexts;
            _nativeContextName = contexts.FirstOrDefault(context => context.Contains("native", StringComparison.OrdinalIgnoreCase));
        }

        internal void SwitchToWebContext(WebViewContext webViewContext)
        {
            _webViewLocatorStrategy.SwitchToWebView(webViewContext);
        }

        internal void SwitchToNativeContext()
        {
            _contextAwareDriver.Context = _nativeContextName;
        }

        internal void ForEachWebView(Action<string> action)
        {
            _webViewLocatorStrategy.ForEachWebView(action);
        }
    }
}