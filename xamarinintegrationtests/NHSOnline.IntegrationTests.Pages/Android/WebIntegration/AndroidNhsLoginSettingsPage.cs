using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidNhsLoginSettingsPage
    {
        private readonly IAndroidDriverWrapper _driver;
        private AndroidNhsLoginSettingsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NhsLoginSettingsPageContent(driver.Web(WebViewContext.NhsLoginSettingsWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        public NhsLoginSettingsPageContent PageContent { get; }

        public static AndroidNhsLoginSettingsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNhsLoginSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidNhsLoginSettingsPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}