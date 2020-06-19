using System;
using System.Linq;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class NativeDriverContext
    {
        private readonly IContextAware _contextAwareDriver;
        private readonly string _webContextName;
        private readonly string _nativeContextName;

        internal NativeDriverContext(IContextAware contextAwareDriver)
        {
            _contextAwareDriver = contextAwareDriver;
            var contexts = contextAwareDriver.Contexts;
            _webContextName = contexts.FirstOrDefault(context => context.Contains("webview", StringComparison.OrdinalIgnoreCase));
            _nativeContextName = contexts.FirstOrDefault(context => context.Contains("native", StringComparison.OrdinalIgnoreCase));
        }

        internal IDisposable Web() => new WebContext(_contextAwareDriver, _webContextName, _nativeContextName);

        private sealed class WebContext : IDisposable
        {
            private readonly IContextAware _contextAwareDriver;
            private readonly string _nativeContextName;

            public WebContext(IContextAware contextAwareDriver, string webContextName, string nativeContextName)
            {
                _contextAwareDriver = contextAwareDriver;
                _nativeContextName = nativeContextName;

                _contextAwareDriver.Context = webContextName;
            }

            public void Dispose() => _contextAwareDriver.Context = _nativeContextName;
        }
    }
}