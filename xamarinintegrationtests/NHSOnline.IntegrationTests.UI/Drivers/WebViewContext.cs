using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public abstract class WebViewContext
    {
        public abstract string ContextIdentifier { get; }

        // Contexts that will not be returned to within the test
        // but the web view may persist and should not be reused.
        public static WebViewContext OneOff { get; } = new OneOffWebViewContext();
        public static WebViewContext NhsLogin { get; } = new NhsLoginWebViewContext();
        public static WebViewContext NhsAppPreHome { get; } = new NhsAppPreHomeWebViewContext();
        public static WebViewContext NhsLoginUplift { get; } = new NhsLoginUpliftWebViewContext();
        public static WebViewContext NhsApp { get; } = new NhsAppWebViewContext();
        public static WebViewContext ErsWebIntegration { get; } = new ErsWebIntegrationWebViewContext();
        public static WebViewContext DeepLinkLauncher { get; } = new DeepLinkLauncherWebViewContext();
        public static WebViewContext PkbWebIntegration { get; } = new PkbWebIntegrationWebViewContext();
        public static WebViewContext SubstraktWebIntegration { get; } = new SubstraktWebIntegrationWebViewContext();
        public static WebViewContext AToZWebIntegration { get; } = new AToZWebIntegrationWebViewContext();
        public static WebViewContext OneOneOneWebIntegration { get; } = new OneOneOneWebIntegrationWebViewContext();
        public static WebViewContext TestProviderWebIntegration { get; } = new TestWebIntegrationProviderWebViewContext();
        public static WebViewContext NhsLoginSettingsWebIntegration { get; } = new NhsLoginSettingsWebIntegrationWebViewContext();

        internal abstract void AssertContextReady(IWebDriver driver);

        private abstract class NhsAppBaseWebViewContext : WebViewContext
        {
            public override string ContextIdentifier => "webview_com";
        }

        private abstract class ChromeBaseWebViewContext : WebViewContext
        {
            public override string ContextIdentifier => "webview_chrome";
        }

        private class OneOffWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class NhsLoginUpliftWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class NhsLoginWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
                driver.Url.Should().NotBe("about:blank");
            }
        }

        private class NhsAppWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
                Assert.IsTrue(driver.ExecuteJavaScript<bool>(
                    "return window.nhsAppPageLoadComplete === true;"),
                    "window.nhsAppPageLoadComplete was not found to be true");
            }
        }

        private class NhsAppPreHomeWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
                Assert.IsTrue(driver.ExecuteJavaScript<bool>(
                        "return window.nhsAppPageLoadComplete === true;"),
                    "window.nhsAppPageLoadComplete was not found to be true");
            }
        }

        private class ErsWebIntegrationWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class DeepLinkLauncherWebViewContext : ChromeBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class PkbWebIntegrationWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class SubstraktWebIntegrationWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class TestWebIntegrationProviderWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class OneOneOneWebIntegrationWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class AToZWebIntegrationWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }

        private class NhsLoginSettingsWebIntegrationWebViewContext : NhsAppBaseWebViewContext
        {
            internal override void AssertContextReady(IWebDriver driver)
            {
            }
        }
    }
}
