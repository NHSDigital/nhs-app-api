using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSNhsLoginSettingsPage
    {
        private IOSNhsLoginSettingsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NhsLoginSettingsPageContent(driver.Web(WebViewContext.NhsLoginSettingsWebIntegration));
        }

        public IOSFullNavigation Navigation { get; }

        private NhsLoginSettingsPageContent PageContent { get; }

        public static IOSNhsLoginSettingsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNhsLoginSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSNhsLoginSettingsPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}