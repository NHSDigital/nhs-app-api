using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public sealed class AndroidYourHealthPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidYourHealthPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new YourHealthPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public YourHealthPageContent PageContent { get; }

        public static AndroidYourHealthPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}
