using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSWebIntegrationWarningPanelPage
    {
        private IOSWebIntegrationWarningPanelPage(IIOSDriverWrapper driver, string pageTitle)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new WebIntegrationWarningPanelPageContent(driver.Web.NhsAppLoggedInWebView(), pageTitle);
        }

        private IOSFullNavigation Navigation { get; }

        public WebIntegrationWarningPanelPageContent PageContent { get; }

        public static IOSWebIntegrationWarningPanelPage AssertOnPage(IIOSDriverWrapper driver, string pageTitle)
        {
            var page = new IOSWebIntegrationWarningPanelPage(driver, pageTitle);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSWebIntegrationWarningPanelPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}