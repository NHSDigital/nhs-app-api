using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Components.IOS;
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

        private IOSRadioButton OptInToUserResearchRadioButton =>
            IOSRadioButton.StartsWith(_driver, "Yes, you can contact me about taking part in user research");

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

        public IOSUserResearchOptInPage OptInToUserResearch()
        {
            PageContent.ContinueButton.ScrollTo();
            OptInToUserResearchRadioButton.Click();
            PageContent.ContinueButton.Click();

            return this;
        }

        public void ScreenshotError() => _driver.Screenshot($"{nameof(IOSUserResearchOptInPage)}_error");
    }
}