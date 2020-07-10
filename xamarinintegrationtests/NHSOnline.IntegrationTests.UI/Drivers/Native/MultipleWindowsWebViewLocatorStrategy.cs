using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class MultipleWindowsWebViewLocatorStrategy<TDriver> : WebViewLocatorStrategy
        where TDriver: class, IWebDriver, IContextAware
    {
        private readonly WebViewContextsCache _contextsCache = new WebViewContextsCache();

        private readonly TDriver _driver;

        public MultipleWindowsWebViewLocatorStrategy(TDriver driver) => _driver = driver;

        internal override void ForEachWebView(Action<string> action)
        {
            if (TryGetWebContextName(out var webContextName))
            {
                _driver.Context = webContextName;
                foreach (var windowHandle in _driver.WindowHandles)
                {
                    _driver.SwitchTo().Window(windowHandle);
                    action(windowHandle);
                }
            }
        }

        internal override void SwitchToWebView(WebViewContext webViewContext)
        {
            var contextName = WaitForWebViewContextToExist();
            _driver.Context = contextName;

            var windowHandle = WaitForWindowHandleToExist(webViewContext);
            _driver.SwitchTo().Window(windowHandle);
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
            throw new AssertFailedException($"Web context not found after {WaitForWebContext}; Contexts: {string.Join(", ", _driver.Contexts)}");
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
            throw new AssertFailedException($"Web window handle not found after {WaitForWebContext}; Handles: {string.Join(", ", _driver.WindowHandles)}");
        }

        private bool TryGetWebContextName([NotNullWhen(true)] out string? webContextName)
        {
            webContextName = _driver.Contexts.FirstOrDefault(IsWebViewContext);
            return webContextName != null;
        }

        private bool TryGetWindowHandle([NotNullWhen(true)] out string? windowHandle)
        {
            windowHandle = _driver.WindowHandles.Except(_contextsCache.Contexts).FirstOrDefault();
            return windowHandle != null;
        }
    }
}