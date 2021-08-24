using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings
{
    public sealed class AndroidAccountSettingsPage
    {
        private readonly IAndroidDriverWrapper _driver;
        private AndroidAccountSettingsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AccountSettingsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public AccountSettingsPageContent PageContent { get; }

        internal AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            GetAllKeyboardMoreNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardMoreNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public static AndroidAccountSettingsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAccountSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidAccountSettingsPage AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void KeyboardNavigateToBiometricsSettings() =>
            PageContent.KeyboardNavigateToBiometricsSettings(KeyboardPageContentNavigation);

        public void KeyboardNavigateToNhsLoginSettings() =>
            PageContent.KeyboardNavigateToNhsLoginSettings(KeyboardPageContentNavigation);

        public void KeyboardNavigateToNotifications() =>
            PageContent.KeyboardNavigateToNotificationsSettings(KeyboardPageContentNavigation);

        public void KeyboardNavigateToCookiesSettings() =>
            PageContent.KeyboardNavigateToCookiesSettings(KeyboardPageContentNavigation);
    }
}
