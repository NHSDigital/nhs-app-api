using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidLoggedInHomePage
    {
        public AndroidFullNavigation Navigation { get; }
        public LoggedInHomePageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedInHomePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new LoggedInHomePageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static AndroidLoggedInHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedInHomePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageDisplayedFor(string name)
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertNameDisplayedFor(name);
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

        public void KeyboardNavigateToAdvice() => Navigation.KeyboardNavigateToAdvice(KeyboardPageContentNavigation);

        public void KeyboardNavigateToAppointments() => Navigation.KeyboardNavigateToAppointments(KeyboardPageContentNavigation);

        public void KeyboardNavigatePrescriptions() => Navigation.KeyboardNavigateToPrescriptions(KeyboardPageContentNavigation);

        public void KeyboardNavigateToYourHealth() => Navigation.KeyboardNavigateToYourHealth(KeyboardPageContentNavigation);

        public void KeyboardNavigateToMessages() => Navigation.KeyboardNavigateToMessages(KeyboardPageContentNavigation);

        public void KeyboardNavigateToHelp() => Navigation.KeyboardNavigateToHelp(KeyboardPageContentNavigation);

        public void KeyboardNavigateToMore() => Navigation.KeyboardNavigateToMore(KeyboardPageContentNavigation);
    }
}