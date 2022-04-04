using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidTermsAndConditionsPage
    {
        public TermsAndConditionsPageContent PageContent { get; }

        private IAndroidDriverWrapper _driver { get; }

        private AndroidTermsAndConditionsPage(IAndroidDriverWrapper driver)
        {
            PageContent = new TermsAndConditionsPageContent(driver.Web.NhsAppPreHomeWebView());
            _driver = driver;
        }

        public static AndroidTermsAndConditionsPage AssertOnPage(IAndroidDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new AndroidTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidTermsAndConditionsPage));
            }

            return page;
        }

        public void AssertPageContent()
        {
            PageContent.AssertPageContent();
        }

        public void ScrollToContinueAndScreenshot()
        {
            PageContent.ContinueButton.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidTermsAndConditionsPage)}_scrolled");
        }

        public void ScreenshotError() => _driver.Screenshot($"{nameof(AndroidTermsAndConditionsPage)}_error");
    }
}