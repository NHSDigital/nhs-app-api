
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public class IOSOrderARepeatPrescriptionPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFullNavigation Navigation { get; }

        public OrderARepeatPrescriptionPageContent PageContent { get; }


        private IOSOrderARepeatPrescriptionPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new OrderARepeatPrescriptionPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSOrderARepeatPrescriptionPage AssertOnPage(IIOSDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new IOSOrderARepeatPrescriptionPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSOrderARepeatPrescriptionPage));
            }

            return page;
        }

        public void ScreenshotError()
        {
            _driver.Screenshot($"{nameof(IOSOrderARepeatPrescriptionPage)}_error");
        }
    }
}