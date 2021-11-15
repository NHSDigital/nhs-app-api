using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Messages;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.Messages
{
    public sealed class AndroidMessagesPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidMessagesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new MessagesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            GetAllKeyboardMessagesNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardMessagesNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public AndroidFullNavigation Navigation { get; }

        public MessagesPageContent PageContent { get; }

        public static AndroidMessagesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            // Extending timeout to allow page to finish reloading
            using var extendedTimeout = ExtendedTimeout.FromSeconds(5);

            var page = new AndroidMessagesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidMessagesPage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }

        public void KeyboardNavigateToGpSurgeryMessages() => PageContent.KeyboardNavigateToGpSurgeryMessages(KeyboardPageContentNavigation);

        public void KeyboardNavigateToSubstrakt() => PageContent.KeyboardNavigateToSubstrakt(KeyboardPageContentNavigation);

        public void KeyboardNavigateToGncr() => PageContent.KeyboardNavigateToGncr(KeyboardPageContentNavigation);

        public void KeyboardNavigateToPkb() => PageContent.KeyboardNavigateToPkb(KeyboardPageContentNavigation);

        public void KeyboardNavigateToCie() => PageContent.KeyboardNavigateToCie(KeyboardPageContentNavigation);

        public void KeyboardNavigateToMyCareView() => PageContent.KeyboardNavigateToMyCareView(KeyboardPageContentNavigation);

        public void KeyboardNavigateToSecondaryCareView() => PageContent.KeyboardNavigateToSecondaryCareView(KeyboardPageContentNavigation);

        public void KeyboardNavigateToYourHealthServiceMessages() => PageContent.KeyboardNavigateToYourHealthServiceMessages(KeyboardPageContentNavigation);
    }
}
