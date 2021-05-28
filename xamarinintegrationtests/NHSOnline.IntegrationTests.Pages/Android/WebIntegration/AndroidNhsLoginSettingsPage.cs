using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidNhsLoginSettingsPage
    {
        public AndroidFullNavigation Navigation { get; }
        private NhsLoginSettingsPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidNhsLoginSettingsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NhsLoginSettingsPageContent(driver.Web(WebViewContext.NhsLoginSettingsWebIntegration));
        }

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

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllFocusableElements());

        private IEnumerable<IFocusable> GetAllFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();

            return footerFocusableList.Concat(headerFocusableList);
        }

        public void KeyboardNavigateToAdvice() => Navigation.KeyboardNavigateToAdvice(KeyboardPageContentNavigation);
    }
}