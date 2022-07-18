using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public class IOSOrderARepeatPrescriptionErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFullNavigation Navigation { get; }

        private OrderARepeatPrescriptionErrorPageContent PageContent { get; }

        private IOSOrderARepeatPrescriptionErrorPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new OrderARepeatPrescriptionErrorPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSOrderARepeatPrescriptionErrorPage AssertOnPage(IIOSDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new IOSOrderARepeatPrescriptionErrorPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSOrderARepeatPrescriptionErrorPage));
            }

            return page;
        }

        public void ClickTryAgainButton() => PageContent.ClickTryAgainButton();
    }
}