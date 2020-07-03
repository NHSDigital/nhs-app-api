using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class MultipleContextsWebViewLocatorStrategy : WebViewLocatorStrategy
    {
        private readonly Dictionary<WebViewContext, string> _knownContexts = new Dictionary<WebViewContext, string>();

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
            if (_knownContexts.TryGetValue(webViewContext, out var knownWebViewContext) &&
                _driver.Contexts.Contains(knownWebViewContext))
            {
                return knownWebViewContext;
            }

            var waitUtil = DateTime.UtcNow.Add(WaitForWebContext);
            while (DateTime.UtcNow < waitUtil)
            {
                if (TryGetWebContextName(out var webContextName))
                {
                    _knownContexts[webViewContext] = webContextName;
                    return webContextName;
                }
            }
            throw new AssertFailedException($"Web context not found after {WaitForWebContext}; Contexts: {string.Join(", ", _driver.Contexts)}");
        }

        private bool TryGetWebContextName([NotNullWhen(true)] out string? webContextName)
        {
            webContextName = _driver.Contexts.Except(_knownContexts.Values).FirstOrDefault(IsWebViewContext);
            return webContextName != null;
        }
    }
}