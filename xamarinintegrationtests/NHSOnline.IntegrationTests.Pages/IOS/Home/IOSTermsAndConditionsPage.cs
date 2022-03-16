using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSTermsAndConditionsPage
    {
        public TermsAndConditionsPageContent PageContent { get; }

        private IIOSDriverWrapper _driver { get; }

        private IOSTermsAndConditionsPage(IIOSDriverWrapper driver)
        {
            PageContent = new TermsAndConditionsPageContent(driver.Web.NhsAppPreHomeWebView());
            _driver = driver;
        }

        public static IOSTermsAndConditionsPage AssertOnPage(IIOSDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new IOSTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSTermsAndConditionsPage));
            }

            return page;
        }

        public void AssertPageContent()
        {
            PageContent.AssertPageContent();
        }

        public void ScrollAndScreenshot()
        {
            PageContent.ContinueButton.ScrollTo();
            _driver.Screenshot($"{nameof(IOSTermsAndConditionsPage)}_scrolled");
        }
    }
}