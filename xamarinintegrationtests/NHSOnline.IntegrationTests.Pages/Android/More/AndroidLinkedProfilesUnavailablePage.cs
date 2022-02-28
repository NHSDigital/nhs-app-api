using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More
{
    public sealed class AndroidLinkedProfilesUnavailablePage
    {
        private AndroidFullNavigation Navigation { get; }
        public LinkedProfilesUnavailablePageContent PageContent { get; }

        private AndroidLinkedProfilesUnavailablePage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new LinkedProfilesUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidLinkedProfilesUnavailablePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLinkedProfilesUnavailablePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidLinkedProfilesUnavailablePage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();
            
            return this;
        }
    }
}