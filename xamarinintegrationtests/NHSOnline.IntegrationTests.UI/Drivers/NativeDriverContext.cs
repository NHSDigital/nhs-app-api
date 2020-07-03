using System;
using System.Linq;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers
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

        internal IDisposable Web(WebViewContext webViewContext)
        {
            _webViewLocatorStrategy.SwitchToWebView(webViewContext);
            return new RevertWebContext(_contextAwareDriver, _nativeContextName);
        }

        internal void ForEachWebView(Action<string> action)
        {
            using var webContext = new RevertWebContext(_contextAwareDriver, _nativeContextName);

            _webViewLocatorStrategy.ForEachWebView(action);
        }

        private sealed class RevertWebContext : IDisposable
        {
            private readonly IContextAware _contextAwareDriver;
            private readonly string _nativeContextName;

            public RevertWebContext(IContextAware contextAwareDriver, string nativeContextName)
            {
                _contextAwareDriver = contextAwareDriver;
                _nativeContextName = nativeContextName;
            }

            public void Dispose() => _contextAwareDriver.Context = _nativeContextName;
        }
    }
}