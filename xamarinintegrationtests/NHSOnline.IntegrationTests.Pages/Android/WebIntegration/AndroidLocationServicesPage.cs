using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidLocationServicesPage
    {

        private readonly IAndroidDriverWrapper _driver;

        private AndroidLocationServicesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new LocationServicesPageContent(driver.Web(WebViewContext.TestProviderWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        public LocationServicesPageContent PageContent { get; }

        public static AndroidLocationServicesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLocationServicesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidLocationServicesPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }

        public void ShowLocation()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);

            _driver.SendKey(AndroidKeyCode.Keycode_ENTER);
        }
    }
}