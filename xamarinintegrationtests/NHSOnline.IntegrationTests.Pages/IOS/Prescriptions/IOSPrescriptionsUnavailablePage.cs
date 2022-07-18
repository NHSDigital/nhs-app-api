using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public sealed class IOSPrescriptionsUnavailablePage
    {
        private IOSFullNavigation Navigation { get; }
        public PrescriptionsUnavailablePageContent PageContent { get; }

        private IOSPrescriptionsUnavailablePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PrescriptionsUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSPrescriptionsUnavailablePage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSPrescriptionsUnavailablePage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSPrescriptionsUnavailablePage));
            }

            return page;
        }

        public IOSPrescriptionsUnavailablePage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }
    }
}