using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidTestWebIntegrationProviderPage
    {
        public AndroidFullNavigation Navigation { get; }
        public TestWebIntegrationProviderPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidTestWebIntegrationProviderPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new TestWebIntegrationProviderPageContent(driver.Web(WebViewContext.TestProviderWebIntegration));
        }

        public static AndroidTestWebIntegrationProviderPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidTestWebIntegrationProviderPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidTestWebIntegrationProviderPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllFocusableElements());

        private IEnumerable<IFocusable> GetAllFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();

            return footerFocusableList.Concat(headerFocusableList);
        }

        public void KeyboardNavigateToYourHealth() => Navigation.KeyboardNavigateToYourHealth(KeyboardPageContentNavigation);
    }
}