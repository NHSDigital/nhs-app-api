using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Messages;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Messages
{
    public class AndroidYourHealthServiceMessagesPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidYourHealthServiceMessagesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new YourHealthServiceMessagesPageContent(driver.Web.NhsAppLoggedInWebView());
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

        private AndroidFullNavigation Navigation { get; }

        private YourHealthServiceMessagesPageContent PageContent { get; }

        public static AndroidYourHealthServiceMessagesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthServiceMessagesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void KeyboardNavigateBack() => PageContent.KeyboardNavigateBack(KeyboardPageContentNavigation);
    }
}