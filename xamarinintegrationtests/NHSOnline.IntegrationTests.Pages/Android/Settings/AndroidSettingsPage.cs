using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Settings
{
    public sealed class AndroidSettingsPage
    {
        private AndroidSettingsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new SettingsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public AndroidFullNavigation Navigation { get; }

        private SettingsPageContent PageContent { get; }

        public static AndroidSettingsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidSettingsPage AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();

            return this;
        }
    }
}
