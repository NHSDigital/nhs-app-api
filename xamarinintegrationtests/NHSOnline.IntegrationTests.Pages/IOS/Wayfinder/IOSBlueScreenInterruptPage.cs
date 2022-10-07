using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Wayfinder
{
    public class IOSBlueScreenInterruptPage
    {
        private IOSBlueScreenInterruptPage(IIOSDriverWrapper driver)
        {
            PageContent = new BlueScreenInterruptPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private BlueScreenInterruptPageContent PageContent { get; }

        public static void AssertOnPage(IIOSDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new IOSBlueScreenInterruptPage(driver);
            page.PageContent.AssertPageElements();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSSecondaryCareSummaryPage));
            }
        }
    }
}

