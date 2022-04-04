using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidUserResearchOptInPage
    {
        public UserResearchOptInPageContent PageContent { get; }
        private IAndroidDriverWrapper _driver { get; }

        private AndroidUserResearchOptInPage(IAndroidDriverWrapper driver)
        {
            PageContent = new UserResearchOptInPageContent(driver.Web.NhsAppPreHomeWebView());
            _driver = driver;
        }

        public static AndroidUserResearchOptInPage AssertOnPage(IAndroidDriverWrapper driver,
            bool screenshot = false)
        {
            var page = new AndroidUserResearchOptInPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidUserResearchOptInPage));
            }

            return page;
        }

        public void ScreenshotError() => _driver.Screenshot($"{nameof(AndroidUserResearchOptInPage)}_error");
    }
}