using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidRedirectorPage
    {
        private AndroidFullNavigation Navigation { get; }
        private BlankPageContent PageContent { get; }

        private AndroidRedirectorPage(IAndroidDriverWrapper driver)
        {
            PageContent = new BlankPageContent(driver.Web.NhsAppLoggedInWebView());
            Navigation = new AndroidFullNavigation(driver);
        }

        public static void AssertOnPage(IAndroidDriverWrapper driver)
        {
            var androidRedirectorPage = new AndroidRedirectorPage(driver);
            androidRedirectorPage.PageContent.AssertOnPage();
            androidRedirectorPage.Navigation.AssertNavigationIconsArePresent();
        }
    }
}