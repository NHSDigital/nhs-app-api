using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public class IOSPrescriptionConfirmedErrorPage
    {
        private IOSFullNavigation Navigation { get; }

        private PrescriptionConfirmedErrorPageContent PageContent { get; }

        private IOSPrescriptionConfirmedErrorPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PrescriptionConfirmedErrorPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static void AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSPrescriptionConfirmedErrorPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSPrescriptionConfirmedErrorPage));
            }
        }
    }
}