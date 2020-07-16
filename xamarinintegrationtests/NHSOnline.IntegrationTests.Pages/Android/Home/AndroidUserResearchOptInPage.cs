using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidUserResearchOptInPage
    {
        public UserResearchOptInPageContent PageContent { get; }

        private AndroidUserResearchOptInPage(IAndroidDriverWrapper driver)
        {
            PageContent = new UserResearchOptInPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static AndroidUserResearchOptInPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidUserResearchOptInPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}