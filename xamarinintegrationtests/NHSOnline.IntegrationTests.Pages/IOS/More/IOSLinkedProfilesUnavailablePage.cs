using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More
{
    public sealed class IOSLinkedProfilesUnavailablePage
    {
        private IOSFullNavigation Navigation { get; }
        public LinkedProfilesUnavailablePageContent PageContent { get; }

        private IOSLinkedProfilesUnavailablePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new LinkedProfilesUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSLinkedProfilesUnavailablePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLinkedProfilesUnavailablePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSLinkedProfilesUnavailablePage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }
    }
}