using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public class AndroidYourHealthPkbPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidYourHealthPkbPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new YourHealthPkbViewPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public YourHealthPkbViewPageContent PageContent { get; }

        public static AndroidYourHealthPkbPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthPkbPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidYourHealthPkbPage TabIntoFocus()
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

        public void KeyboardNavigateToCovidPass() => PageContent.KeyboardNavigateToCovidPass(KeyboardPageContentNavigation);

        public void KeyboardNavigateToVaccineRecord() => PageContent.KeyboardNavigateToVaccineRecord(KeyboardPageContentNavigation);

        public void KeyboardNavigateToGpHealthRecord() => PageContent.KeyboardNavigateToGpHealthRecord(KeyboardPageContentNavigation);

        public void KeyboardNavigateToTestResults() => PageContent.KeyboardNavigateToTestResults(KeyboardPageContentNavigation);

        public void KeyboardNavigateToCarePlans() => PageContent.KeyboardNavigateToCarePlans(KeyboardPageContentNavigation);

        public void KeyboardNavigateToTrackYourHealth() => PageContent.KeyboardNavigateToTrackYourHealth(KeyboardPageContentNavigation);

        public void KeyboardNavigateToSharedHealth() => PageContent.KeyboardNavigateToSharedHealth(KeyboardPageContentNavigation);

        public void KeyboardNavigateToRecordSharing() => PageContent.KeyboardNavigateToRecordSharing(KeyboardPageContentNavigation);

        public void KeyboardNavigateToOrganDonation() => PageContent.KeyboardNavigateToOrganDonation(KeyboardPageContentNavigation);

        public void KeyboardNavigateToNdop() => PageContent.KeyboardNavigateToNdop(KeyboardPageContentNavigation);
    }
}