using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public class IOSChooseRepeatPrescriptionPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFullNavigation Navigation { get; }

        public ChoosePrescriptionPageContent PageContent { get; }


        private IOSChooseRepeatPrescriptionPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new ChoosePrescriptionPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSChooseRepeatPrescriptionPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSChooseRepeatPrescriptionPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSChooseRepeatPrescriptionPage));
            }

            return page;
        }
    }
}