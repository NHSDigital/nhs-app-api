using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth.Ndop;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth.Ndop
{
    public sealed class AndroidNdopMakeYourChoicePage
    {

        private AndroidNdopMakeYourChoicePage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NdopMakeYourChoicePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public NdopMakeYourChoicePageContent PageContent { get; }

        public static AndroidNdopMakeYourChoicePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNdopMakeYourChoicePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}