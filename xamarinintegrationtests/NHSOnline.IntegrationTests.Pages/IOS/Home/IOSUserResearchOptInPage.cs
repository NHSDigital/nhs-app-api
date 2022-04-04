using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSUserResearchOptInPage
    {
        public UserResearchOptInPageContent PageContent { get; }

        private IIOSDriverWrapper _driver { get; }

        private IOSUserResearchOptInPage(IIOSDriverWrapper driver)
        {
            PageContent = new UserResearchOptInPageContent(driver.Web.NhsAppPreHomeWebView());
            _driver = driver;
        }

        public static IOSUserResearchOptInPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSUserResearchOptInPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSUserResearchOptInPage));
            }

            return page;
        }

        public void ScreenshotError() => _driver.Screenshot($"{nameof(IOSUserResearchOptInPage)}_error");
    }
}