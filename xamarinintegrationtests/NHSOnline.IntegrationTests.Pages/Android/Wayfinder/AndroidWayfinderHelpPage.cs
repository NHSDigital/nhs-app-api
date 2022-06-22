using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.Wayfinder
{
    public class AndroidWayfinderHelpPage
    {
        private AndroidFullNavigation Navigation { get; }
        private WayfinderHelpPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidWayfinderHelpPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new WayfinderHelpPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidWayfinderHelpPage AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidWayfinderHelpPage(driver);

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidSecondaryCareSummaryPage));
            }

            return page;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardHomeNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardHomeNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public void KeyboardNavigateViaBackButton() =>
            PageContent.NavigateViaBackButton(KeyboardPageContentNavigation);

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
         public AndroidWayfinderHelpPage TabIntoFocus()
         {
             _driver.SendKey(AndroidKeyCode.Keycode_TAB);
             return this;
         }
    }
}

