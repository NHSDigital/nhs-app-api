using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More;
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
            PageContent = new MorePageContent(driver.Web.NhsAppLoggedInWebView());
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
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void KeyboardNavigateToAccountAndSettings() =>
            PageContent.KeyboardNavigateToAccountAndSettings(KeyboardPageContentNavigation);

        public void KeyboardNavigateToLogout() =>
            PageContent.KeyboardNavigateToLogOut(KeyboardPageContentNavigation);
    }
}
