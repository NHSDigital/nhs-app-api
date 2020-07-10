using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class MultipleContextsWebViewLocatorStrategy : WebViewLocatorStrategy
    {
        private readonly WebViewContextsCache _contextsCache = new WebViewContextsCache();

        private readonly IContextAware _driver;

        public MultipleContextsWebViewLocatorStrategy(IContextAware driver) => _driver = driver;

        internal override void ForEachWebView(Action<string> action)
        {
            foreach (var contextName in _driver.Contexts.Where(IsWebViewContext))
            {
                _driver.Context = contextName;
                action(contextName);
            }
        }

        internal override void SwitchToWebView(WebViewContext webViewContext)
        {
            var contextName = WaitForWebViewContextToExist(webViewContext);
            _driver.Context = contextName;
        }

        private string WaitForWebViewContextToExist(WebViewContext webViewContext)
        {
            if (_contextsCache.TryGet(webViewContext, out var knownWebViewContext) &&
                _driver.Contexts.Contains(knownWebViewContext))
            {
                return knownWebViewContext;
            }

            var waitUtil = DateTime.UtcNow.Add(WaitForWebContext);
            while (DateTime.UtcNow < waitUtil)
            {
                if (TryGetWebContextName(out var webContextName))
                {
                    _contextsCache.Add(webViewContext, webContextName);
                    return webContextName;
                }
            }
            throw new AssertFailedException($"Web context not found after {WaitForWebContext}; Contexts: {string.Join(", ", _driver.Contexts)}");
        }

        private bool TryGetWebContextName([NotNullWhen(true)] out string? webContextName)
        {
            webContextName = _driver.Contexts.Except(_contextsCache.Contexts).FirstOrDefault(IsWebViewContext);
            return webContextName != null;
        }
    }
}