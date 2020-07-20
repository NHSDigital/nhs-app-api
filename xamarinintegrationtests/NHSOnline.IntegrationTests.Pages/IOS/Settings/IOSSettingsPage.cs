using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Settings
{
    public class IOSSettingsPage
    {
        private IOSSettingsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new SettingsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public IOSFullNavigation Navigation { get; }

        private SettingsPageContent PageContent { get; }

        public static IOSSettingsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSSettingsPage AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();

            return this;
        }
    }
}