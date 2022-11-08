using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Components.IOS;
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

        private IOSRadioButton PrescriptionType => IOSRadioButton.StartsWith(_driver,
             "A repeat prescription");

        private IOSButton ContinueButton => IOSButton.WithText(_driver, "Continue");

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

        public IOSOrderARepeatPrescriptionPage ChooseRepeat()
        {
            PrescriptionType.Click();
            return this;
        }

        public void Continue() => ContinueButton.Click();

        public void ScreenshotError()
        {
            _driver.Screenshot($"{nameof(IOSOrderARepeatPrescriptionPage)}_error");
        }

        public void ScrollToContinueAndScreenshot()
        {
            PageContent.ContinueButton.ScrollTo();
            _driver.Screenshot($"{nameof(IOSOrderARepeatPrescriptionPage)}_scrolled");
        }
    }
}