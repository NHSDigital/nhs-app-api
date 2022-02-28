using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public sealed class AndroidYourHealthUnavailablePage
    {
        private YourHealthUnavailablePageContent PageContent { get; }

        private AndroidYourHealthUnavailablePage(IAndroidDriverWrapper driver)
        {
            PageContent = new YourHealthUnavailablePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static void AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthUnavailablePage(driver);
            page.PageContent.AssertOnPage();
        }
    }
}