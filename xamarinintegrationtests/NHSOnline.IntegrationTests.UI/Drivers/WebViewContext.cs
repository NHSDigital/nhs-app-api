using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public abstract class WebViewContext
    {
        // Contexts that will not be returned to within the test
        // but the web view may persist and should not be reused.
        public static WebViewContext OneOff { get; } = new OneOffWebViewContext();
        public static WebViewContext NhsLogin { get; } = new NhsLoginWebViewContext();
        public static WebViewContext NhsApp { get; } = new NhsAppWebViewContext();
        public static WebViewContext ErsWebIntegration { get; } = new ErsWebIntegrationWebViewContext();

        internal abstract void AssertContextReady(IWebDriver driver);

        private class OneOffWebViewContext : WebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class NhsLoginWebViewContext : WebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class NhsAppWebViewContext : WebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
                Assert.IsTrue(driver.ExecuteJavaScript<bool>(
                    "return window.nhsAppPageLoadComplete === true;"),
                    "window.nhsAppPageLoadComplete was not found to be true");
            }
        }

        private class ErsWebIntegrationWebViewContext : WebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }
    }
}
