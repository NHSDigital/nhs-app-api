using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public sealed class IOSRepeatPrescriptionsUnavailablePage
    {
        private IOSFullNavigation Navigation { get; }
        public RepeatPrescriptionsUnavailablePageContent PageContent { get; }

        private IOSRepeatPrescriptionsUnavailablePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new RepeatPrescriptionsUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSRepeatPrescriptionsUnavailablePage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSRepeatPrescriptionsUnavailablePage(driver);
            page.PageContent.AssertPageElements();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSRepeatPrescriptionsUnavailablePage));
            }

            return page;
        }
    }
}