using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidUserResearchOptInPage
    {
        public UserResearchOptInPageContent PageContent { get; }

        private AndroidUserResearchOptInPage(IAndroidDriverWrapper driver)
        {
            PageContent = new UserResearchOptInPageContent(driver.Web.NhsAppPreHomeWebView());
        }

        public static AndroidUserResearchOptInPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidUserResearchOptInPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}