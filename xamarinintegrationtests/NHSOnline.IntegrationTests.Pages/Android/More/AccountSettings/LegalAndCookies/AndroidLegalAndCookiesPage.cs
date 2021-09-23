using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.LegalAndCookies;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings.LegalAndCookies
{
    public class AndroidLegalAndCookiesPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFullNavigation Navigation { get; }
        private LegalAndCookiesPageContent PageContent { get; }

        private AndroidLegalAndCookiesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;

            Navigation = new AndroidFullNavigation(driver);
            PageContent = new LegalAndCookiesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidLegalAndCookiesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLegalAndCookiesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            GetAllKeyboardNotificationsNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardNotificationsNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public void KeyboardNavigateToManageCookies() => PageContent.KeyboardNavigateToManageCookies(KeyboardPageContentNavigation);

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidLegalAndCookiesPage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }
    }
}