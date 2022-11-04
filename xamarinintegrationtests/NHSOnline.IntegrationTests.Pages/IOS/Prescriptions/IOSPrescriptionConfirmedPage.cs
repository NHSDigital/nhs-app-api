using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public class IOSPrescriptionConfirmedPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFullNavigation Navigation { get; }

        public PrescriptionConfirmedPageContent PageContent { get; }

        private IOSPrescriptionConfirmedPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PrescriptionConfirmedPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static void AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSPrescriptionConfirmedPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSPrescriptionConfirmedPage));
            }
        }
    }
}