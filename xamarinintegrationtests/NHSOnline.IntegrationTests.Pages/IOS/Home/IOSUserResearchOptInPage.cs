using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSUserResearchOptInPage
    {
        public UserResearchOptInPageContent PageContent { get; }

        private IOSUserResearchOptInPage(IIOSDriverWrapper driver)
        {
            PageContent = new UserResearchOptInPageContent(driver.Web.NhsAppPreHomeWebView());
        }

        public static IOSUserResearchOptInPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSUserResearchOptInPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}