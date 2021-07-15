using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers.WebContext
{
    internal class WebViewContextGrabber
    {
        private static TimeSpan WaitForWebContext { get; } = TimeSpan.FromSeconds(20);

        private readonly WebViewLocatorStrategy _webViewLocatorStrategy;
        private readonly TestLogs _logs;
        private readonly HashSet<IWebContext> _usedContexts = new();

        public WebViewContextGrabber(WebViewLocatorStrategy webViewLocatorStrategy, TestLogs logs)
        {
            _webViewLocatorStrategy = webViewLocatorStrategy;
            _logs = logs;
        }

        public IWebContext GrabNextUnusedWebContext(WebContextKind webContextKind)
        {
            return GrabRetrier(() =>
            {
                var allWebContexts = _webViewLocatorStrategy.GetWebContexts(webContextKind);
                _logs.Info($"All web contexts: {string.Join(", ", allWebContexts)}");
                var excludingAlreadyUsed = allWebContexts.Where(context => !_usedContexts.Contains(context));
                var firstAvailable = excludingAlreadyUsed.FirstOrDefault();

                if (firstAvailable == null)
                {
                    throw new AssertFailedException(
                        $"Unused web context not found after {WaitForWebContext}; Contexts: {string.Join(", ", allWebContexts)}");
                }

                _usedContexts.Add(firstAvailable);
                return firstAvailable;
            });
        }

        private static IWebContext GrabRetrier(Func<IWebContext> grabber)
        {
            var retryUntil = DateTime.UtcNow.Add(TimeSpan.FromMinutes(1));
            while (true)
            {
                try
                {
                    return grabber();
                }
                catch (AssertFailedException) when (DateTime.UtcNow < retryUntil)
                {
                }
                catch (WebDriverException) when (DateTime.UtcNow < retryUntil)
                {
                }
                catch (InvalidOperationException e) when (DateTime.UtcNow < retryUntil && IsNoSuchContextFound(e))
                {
                }

                Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            }

            static bool IsNoSuchContextFound(InvalidOperationException e)
                => e.Message.StartsWith("Unused web context not found.", StringComparison.Ordinal);
        }
    }
}