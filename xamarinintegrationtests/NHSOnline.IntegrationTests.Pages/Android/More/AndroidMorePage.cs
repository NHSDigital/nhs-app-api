using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More
{
    public sealed class AndroidMorePage
    {
        private readonly IAndroidDriverWrapper _driver;
        private AndroidMorePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new MorePageContent(driver.Web(WebViewContext.NhsApp));
        }

        public AndroidFullNavigation Navigation { get; }

        public MorePageContent PageContent { get; }

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

        public static AndroidMorePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidMorePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidMorePage AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void KeyboardNavigateToNhsLoginSettings() =>
            PageContent.KeyboardNavigateToNhsLoginSettings(KeyboardPageContentNavigation);

        public void KeyboardNavigateToNotifications() =>
            PageContent.KeyboardNavigateToNotificationSettings(KeyboardPageContentNavigation);
    }
}
