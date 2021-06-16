using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidWebViewLocatorStrategy : WebViewLocatorStrategy
    {
        private readonly WebViewContextsCache _contextsCache = new WebViewContextsCache();

        private readonly AndroidDriver<AndroidElement> _driver;
        private string? _webContextName;

        public AndroidWebViewLocatorStrategy(AndroidDriver<AndroidElement> driver) => _driver = driver;

        internal override void ForEachWebView(Action<string> action)
        {
            foreach (var contextName in GetWebContextNames())
            {
                _driver.Context = contextName;
                foreach (var windowHandle in _driver.WindowHandles)
                {
                    _driver.SwitchTo().Window(windowHandle);
                    action(windowHandle);
                }
            }
        }

        internal override void SwitchToWebView(WebViewContext webViewContext)
        {
            var contextName = WaitForWebViewContextToExist(webViewContext);
            _driver.Context = contextName;

            var windowHandle = WaitForWindowHandleToExist(webViewContext);
            _driver.SwitchTo().Window(windowHandle);
        }

        private string WaitForWebViewContextToExist(WebViewContext webViewContext)
        {
            var waitUtil = DateTime.UtcNow.Add(WaitForWebContext);
            while (DateTime.UtcNow < waitUtil)
            {
                if (IsCurrentContextCorrect(webViewContext.ContextIdentifier))
                {
                    return _webContextName!;
                }

                if (TryGetWebContextName(webViewContext.ContextIdentifier, out var webContextName))
                {
                    return webContextName;
                }
            }

            throw new AssertFailedException(
                $"Web context not found after {WaitForWebContext}; Contexts: {string.Join(", ", _driver.Contexts)}");
        }

        private string WaitForWindowHandleToExist(WebViewContext webViewContext)
        {
            if (_contextsCache.TryGet(webViewContext, out var knownWindowHandle) &&
                _driver.WindowHandles.Contains(knownWindowHandle))
            {
                return knownWindowHandle;
            }

            var waitUtil = DateTime.UtcNow.Add(WaitForWebContext);
            while (DateTime.UtcNow < waitUtil)
            {
                if (TryGetWindowHandle(out var windowHandle))
                {
                    _contextsCache.Add(webViewContext, windowHandle);
                    return windowHandle;
                }
            }

            throw new AssertFailedException(
                $"Web window handle not found after {WaitForWebContext}; Handles: {string.Join(", ", _driver.WindowHandles)}");
        }

        private bool TryGetWebContextName(string contextIdentifier, [NotNullWhen(true)] out string? webContextName)
        {
            _webContextName = _driver.Contexts.FirstOrDefault(x => IsExpectedWebViewContext(x, contextIdentifier));
            webContextName = _webContextName;
            return webContextName != null;
        }

        private static bool IsExpectedWebViewContext(string context, string expectedIdentifier)
            => context.Contains(expectedIdentifier, StringComparison.OrdinalIgnoreCase);

        private bool IsCurrentContextCorrect(string contextIdentifier)
            => _webContextName != null && IsExpectedWebViewContext(_webContextName, contextIdentifier);

        private IEnumerable<string> GetWebContextNames()
            => _driver.Contexts.Where(IsWebViewContext);

        private bool TryGetWindowHandle([NotNullWhen(true)] out string? windowHandle)
        {
            windowHandle = _driver.WindowHandles.Except(_contextsCache.Contexts).FirstOrDefault();
            return windowHandle != null;
        }
    }
}