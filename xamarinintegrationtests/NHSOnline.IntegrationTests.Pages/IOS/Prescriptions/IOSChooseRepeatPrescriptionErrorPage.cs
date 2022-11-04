using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public class IOSChooseRepeatPrescriptionErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFullNavigation Navigation { get; }

        private ChoosePrescriptionErrorPageContent PageContent { get; }

        private IOSChooseRepeatPrescriptionErrorPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new ChoosePrescriptionErrorPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSChooseRepeatPrescriptionErrorPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSChooseRepeatPrescriptionErrorPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSChooseRepeatPrescriptionErrorPage));
            }

            return page;
        }
    }
}