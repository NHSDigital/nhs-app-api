using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings
{
    public sealed class AndroidNotificationsPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidNotificationsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NotificationsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public NotificationsPageContent PageContent { get; }

        internal AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            GetAllKeyboardNotificationsNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardNotificationsNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public static AndroidNotificationsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNotificationsPage(driver);

            KnownIssue.BrowserStackGoogleServicesFailure()
                .ShouldExpect(() =>
                {
                    page.PageContent.AssertOnPage();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    page.PageContent.AssertErrorOnPage();
                });

            return page;
        }

        public static AndroidNotificationsPage AssertErrorOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNotificationsPage(driver);
            page.PageContent.AssertErrorOnPage();
            return page;
        }

        public static AndroidNotificationsPage AssertCannotChangeNotificationsChoiceErrorOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNotificationsPage(driver);
            page.PageContent.AssertNotificationsChoiceErrorOnPage();
            return page;
        }

        public AndroidNotificationsPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();
            return this;
        }

        public void KeyboardNavigateToDeviceSettings() =>
            PageContent.KeyboardNavigateToDeviceSettings(KeyboardPageContentNavigation);
    }
}
