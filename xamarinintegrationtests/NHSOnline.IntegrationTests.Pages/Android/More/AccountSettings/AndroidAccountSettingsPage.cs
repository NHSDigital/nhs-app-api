using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings
{
    public sealed class AndroidAccountSettingsPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFullNavigation Navigation { get; }
        public AccountSettingsPageContent PageContent { get; }

        private AndroidAccountSettingsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;

            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AccountSettingsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAccountSettingsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAccountSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            GetAllKeyboardMoreNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardMoreNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public void KeyboardNavigateToCookiesSettings() => PageContent.KeyboardNavigateToCookiesSettings(KeyboardPageContentNavigation);

        public void NavigateToFingerprintFaceIrisBiometrics() => PageContent.NavigateToBiometricsSettings();

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidAccountSettingsPage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }
    }
}
