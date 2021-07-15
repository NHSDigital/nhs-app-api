using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers.WebContext;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class NativeDriverContext
    {
        private readonly IContextAware _contextAwareDriver;
        private readonly IWebDriver _webDriver;
        private readonly WebViewLocatorStrategy _webViewLocatorStrategy;
        private readonly string _nativeContextName;
        private CurrentDriverContext _currentDriverContext;

        internal TestLogs Logs { get; }
        internal WebViewContextGrabber WebContextGrabber { get; }

        private static TimeSpan WaitForWebContextToBeReady { get; } = TimeSpan.FromSeconds(60);

        internal NativeDriverContext(IContextAware contextAwareDriver, IWebDriver webDriver,
            WebViewLocatorStrategy webViewLocatorStrategy, TestLogs logs)
        {
            _contextAwareDriver = contextAwareDriver;
            _webDriver = webDriver;
            _webViewLocatorStrategy = webViewLocatorStrategy;
            Logs = logs;
            WebContextGrabber = new WebViewContextGrabber(webViewLocatorStrategy, logs);

            var contexts = contextAwareDriver.Contexts;
            _nativeContextName = contexts
                .FirstOrDefault(context => context.Contains("native", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("No native context found");
            _currentDriverContext = CurrentDriverContext.Create();
        }

        internal void SwitchToWebContext(IWebContext webViewContext, Action<IWebDriver> assertReady)
        {
            Logs.Info($"Switching to web context: {webViewContext}");
            _currentDriverContext = _currentDriverContext.SwitchToWebContext(_webViewLocatorStrategy, webViewContext);

            var waitUtil = DateTime.UtcNow.Add(WaitForWebContextToBeReady);
            while (DateTime.UtcNow < waitUtil)
            {
                try
                {
                    Logs.Info($"Testing if {webViewContext} is ready");
                    assertReady(_webDriver);
                    Logs.Info($"{webViewContext} is ready");
                    return;
                }
                catch
                {
                    Logs.Info($"{webViewContext} is not ready");
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }

            throw new AssertFailedException($"Web context not ready after {WaitForWebContextToBeReady}");
        }

        internal void SwitchToNativeContext()
        {
            Logs.Info($"Switching to native context: {_nativeContextName}");
            _currentDriverContext = _currentDriverContext.SwitchToNativeContext(_contextAwareDriver, _nativeContextName);
        }

        internal void ForEachWebView(Action<IWebContext> action)
        {
            _webViewLocatorStrategy.ForEachWebContext(action);
        }
    }
}