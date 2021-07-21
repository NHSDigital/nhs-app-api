using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal abstract class CurrentDriverContext
    {
        internal abstract CurrentDriverContext SwitchToNativeContext(IContextAware nativeDriverContext,
            string nativeContextName);
        internal abstract CurrentDriverContext SwitchToWebContext(WebViewLocatorStrategy nativeDriverContext,
            IWebContext webViewContext);

        private CurrentDriverContext()
        {
        }

        internal static CurrentDriverContext Create(TestLogs logs)
        {
            return new CurrentlyNativeDriverContext(logs);
        }


        private sealed class CurrentlyNativeDriverContext : CurrentDriverContext
        {
            private readonly TestLogs _logs;

            public CurrentlyNativeDriverContext(TestLogs logs)
            {
                _logs = logs;
            }

            internal static CurrentDriverContext SwitchTo(TestLogs logs, IContextAware contextAwareDriver, string nativeContextName)
            {
                logs.Info($"Switching to native context: {nativeContextName}");

                contextAwareDriver.Context = nativeContextName;
                return new CurrentlyNativeDriverContext(logs);
            }

            internal override CurrentDriverContext SwitchToNativeContext(IContextAware nativeDriverContext,
                string nativeContextName)
            {
                return this;
            }

            internal override CurrentDriverContext SwitchToWebContext(WebViewLocatorStrategy webViewLocatorStrategy,
                IWebContext webViewContext)
            {
                return CurrentlyWebDriverContext.SwitchTo(_logs, webViewLocatorStrategy, webViewContext);
            }
        }

        private sealed class CurrentlyWebDriverContext : CurrentDriverContext
        {
            private readonly IWebContext _currentWebContext;
            private readonly TestLogs _logs;

            private CurrentlyWebDriverContext(TestLogs logs, IWebContext webViewContext)
            {
                _logs = logs;
                _currentWebContext = webViewContext;
            }

            internal static CurrentDriverContext SwitchTo(TestLogs logs, WebViewLocatorStrategy webViewLocatorStrategy, IWebContext webViewContext)
            {
                logs.Info($"Switching to web context: {webViewContext}");

                webViewLocatorStrategy.SwitchToWebView(webViewContext);
                return new CurrentlyWebDriverContext(logs, webViewContext);
            }

            internal override CurrentDriverContext SwitchToNativeContext(IContextAware contextAwareDriver,
                string nativeContextName)
            {
                return CurrentlyNativeDriverContext.SwitchTo(_logs, contextAwareDriver, nativeContextName);
            }

            internal override CurrentDriverContext SwitchToWebContext(WebViewLocatorStrategy webViewLocatorStrategy,
                IWebContext webViewContext)
            {
                if (_currentWebContext == webViewContext)
                {
                    return this;
                }

                return SwitchTo(_logs, webViewLocatorStrategy, webViewContext);
            }
        }
    }
}