using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public class AndroidYourHealthCiePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidYourHealthCiePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new YourHealthCieViewPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public YourHealthCieViewPageContent PageContent { get; }

        public static AndroidYourHealthCiePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthCiePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidYourHealthCiePage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }

        private IEnumerable<IFocusable> GetAllKeyboardNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardNavigationFocusableElements());

        public void KeyboardNavigateToTestResults() => PageContent.KeyboardNavigateToTestResults(KeyboardPageContentNavigation);

        public void KeyboardNavigateToCarePlans() => PageContent.KeyboardNavigateToCarePlans(KeyboardPageContentNavigation);

        public void KeyboardNavigateToTrackYourHealth() => PageContent.KeyboardNavigateToTrackYourHealth(KeyboardPageContentNavigation);

        public void KeyboardNavigateToSharedHealth() => PageContent.KeyboardNavigateToSharedHealth(KeyboardPageContentNavigation);

        public void KeyboardNavigateToRecordSharing() => PageContent.KeyboardNavigateToRecordSharing(KeyboardPageContentNavigation);
    }
}