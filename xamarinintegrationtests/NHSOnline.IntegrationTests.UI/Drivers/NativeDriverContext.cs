using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class NativeDriverContext
    {
        private static readonly TimeSpan WaitForWebContext = TimeSpan.FromSeconds(5);

        private readonly IContextAware _contextAwareDriver;

        private readonly string _nativeContextName;

        internal NativeDriverContext(IContextAware contextAwareDriver)
        {
            _contextAwareDriver = contextAwareDriver;
            var contexts = contextAwareDriver.Contexts;
            _nativeContextName = contexts.FirstOrDefault(context => context.Contains("native", StringComparison.OrdinalIgnoreCase));
        }

        internal IDisposable Web()
        {
            var webContextName = WaitForWebViewContextToExist();
            return new WebContext(_contextAwareDriver, _nativeContextName, webContextName);
        }

        internal bool TryWeb([NotNullWhen(true)] out IDisposable? webContext)
        {
            if (TryGetWebContextName(out var webContextName))
            {
                webContext = new WebContext(_contextAwareDriver, _nativeContextName, webContextName);
                return true;
            }

            webContext = null;
            return false;
        }

        private string WaitForWebViewContextToExist()
        {
            var waitUtil = DateTime.UtcNow.Add(WaitForWebContext);
            while (DateTime.UtcNow < waitUtil)
            {
                if (TryGetWebContextName(out var webContextName))
                {
                    return webContextName;
                }
            }
            throw new AssertFailedException($"Web context not found after {WaitForWebContext}");
        }

        private bool TryGetWebContextName([NotNullWhen(true)] out string? webContextName)
        {
            webContextName = _contextAwareDriver.Contexts.FirstOrDefault(IsWebViewContext);
            return webContextName != null;
        }

        private static bool IsWebViewContext(string context)
            => context.Contains("webview", StringComparison.OrdinalIgnoreCase);

        private sealed class WebContext : IDisposable
        {
            private readonly IContextAware _contextAwareDriver;
            private readonly string _nativeContextName;

            public WebContext(IContextAware contextAwareDriver, string nativeContextName, string webContextName)
            {
                _contextAwareDriver = contextAwareDriver;
                _nativeContextName = nativeContextName;

                _contextAwareDriver.Context = webContextName;
            }

            public void Dispose() => _contextAwareDriver.Context = _nativeContextName;
        }
    }
}