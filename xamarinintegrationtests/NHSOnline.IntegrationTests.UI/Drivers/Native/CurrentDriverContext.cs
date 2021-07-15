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

        internal static CurrentDriverContext Create()
        {
            return new CurrentlyNativeDriverContext();
        }


        private sealed class CurrentlyNativeDriverContext : CurrentDriverContext
        {
            internal static CurrentDriverContext SwitchTo(IContextAware contextAwareDriver, string nativeContextName)
            {
                contextAwareDriver.Context = nativeContextName;
                return new CurrentlyNativeDriverContext();
            }

            internal override CurrentDriverContext SwitchToNativeContext(IContextAware nativeDriverContext,
                string nativeContextName)
            {
                return this;
            }

            internal override CurrentDriverContext SwitchToWebContext(WebViewLocatorStrategy webViewLocatorStrategy,
                IWebContext webViewContext)
            {
                return CurrentlyWebDriverContext.SwitchTo(webViewLocatorStrategy, webViewContext);
            }
        }

        private sealed class CurrentlyWebDriverContext : CurrentDriverContext
        {
            private readonly IWebContext _currentWebContext;

            private CurrentlyWebDriverContext(IWebContext webViewContext)
            {
                _currentWebContext = webViewContext;
            }

            internal static CurrentDriverContext SwitchTo(WebViewLocatorStrategy webViewLocatorStrategy, IWebContext webViewContext)
            {
                webViewLocatorStrategy.SwitchToWebView(webViewContext);
                return new CurrentlyWebDriverContext(webViewContext);
            }

            internal override CurrentDriverContext SwitchToNativeContext(IContextAware contextAwareDriver,
                string nativeContextName)
            {
                return CurrentlyNativeDriverContext.SwitchTo(contextAwareDriver, nativeContextName);
            }

            internal override CurrentDriverContext SwitchToWebContext(WebViewLocatorStrategy webViewLocatorStrategy,
                IWebContext webViewContext)
            {
                if (_currentWebContext == webViewContext)
                {
                    return this;
                }

                return SwitchTo(webViewLocatorStrategy, webViewContext);
            }
        }
    }
}