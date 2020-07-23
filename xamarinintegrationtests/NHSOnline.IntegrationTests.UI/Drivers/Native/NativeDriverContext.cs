using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        internal NativeDriverContext(IContextAware contextAwareDriver, IWebDriver webDriver, WebViewLocatorStrategy webViewLocatorStrategy)
        {
            _contextAwareDriver = contextAwareDriver;
            _webDriver = webDriver;
            _webViewLocatorStrategy = webViewLocatorStrategy;

            var contexts = contextAwareDriver.Contexts;
            _nativeContextName = contexts.FirstOrDefault(context => context.Contains("native", StringComparison.OrdinalIgnoreCase));
            _currentDriverContext = CurrentDriverContext.Create(this);
        }

        internal void SwitchToWebContext(WebViewContext webViewContext)
        {
            _currentDriverContext = SwitchContextWithRetry(() => _currentDriverContext.SwitchToWebContext(webViewContext));
        }

        internal void SwitchToNativeContext()
        {
            _currentDriverContext = SwitchContextWithRetry(() => _currentDriverContext.SwitchToNativeContext());
        }

        internal void ForEachWebView(Action<string> action)
        {
            _webViewLocatorStrategy.ForEachWebView(action);
        }

        private static CurrentDriverContext SwitchContextWithRetry(Func<CurrentDriverContext> contextSwitchAction)
        {

            var retryUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(10));
            while (true)
            {
                try
                {
                    return contextSwitchAction();
                }
                catch (AssertFailedException) when (DateTime.UtcNow < retryUntil)
                {
                }
                catch (WebDriverException) when (DateTime.UtcNow < retryUntil)
                {
                }

                Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            }
        }


        private abstract class CurrentDriverContext
        {
            internal abstract CurrentDriverContext SwitchToNativeContext();
            internal abstract CurrentDriverContext SwitchToWebContext(WebViewContext webViewContext);

            internal static CurrentDriverContext Create(NativeDriverContext nativeDriverContext)
            {
                return new CurrentlyNativeDriverContext(nativeDriverContext);
            }

            private sealed class CurrentlyNativeDriverContext : CurrentDriverContext
            {
                private readonly NativeDriverContext _nativeDriverContext;

                internal CurrentlyNativeDriverContext(NativeDriverContext nativeDriverContext)
                {
                    _nativeDriverContext = nativeDriverContext;
                }

                internal static CurrentDriverContext SwitchTo(NativeDriverContext nativeDriverContext)
                {
                    nativeDriverContext._contextAwareDriver.Context = nativeDriverContext._nativeContextName;
                    return new CurrentlyNativeDriverContext(nativeDriverContext);
                }

                internal override CurrentDriverContext SwitchToNativeContext()
                {
                    return this;
                }

                internal override CurrentDriverContext SwitchToWebContext(WebViewContext webViewContext)
                {
                    return CurrentlyWebDriverContext.SwitchTo(_nativeDriverContext, webViewContext);
                }
            }

            private sealed class CurrentlyWebDriverContext : CurrentDriverContext
            {
                private readonly NativeDriverContext _nativeDriverContext;
                private readonly WebViewContext _currentWebViewContext;

                private CurrentlyWebDriverContext(NativeDriverContext nativeDriverContext, WebViewContext currentWebViewContext)
                {
                    _nativeDriverContext = nativeDriverContext;
                    _currentWebViewContext = currentWebViewContext;
                }

                internal static CurrentDriverContext SwitchTo(NativeDriverContext nativeDriverContext, WebViewContext webViewContext)
                {
                    nativeDriverContext._webViewLocatorStrategy.SwitchToWebView(webViewContext);
                    webViewContext.AssertContextReady(nativeDriverContext._webDriver);
                    return new CurrentlyWebDriverContext(nativeDriverContext, webViewContext);
                }

                internal override CurrentDriverContext SwitchToNativeContext()
                {
                    return CurrentlyNativeDriverContext.SwitchTo(_nativeDriverContext);
                }

                internal override CurrentDriverContext SwitchToWebContext(WebViewContext webViewContext)
                {
                    if (_currentWebViewContext == webViewContext)
                    {
                        return this;
                    }

                    return SwitchTo(_nativeDriverContext, webViewContext);
                }
            }
        }
    }
}