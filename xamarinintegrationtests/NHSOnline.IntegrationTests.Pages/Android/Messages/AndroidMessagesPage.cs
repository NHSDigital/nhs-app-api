using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Messages
{
    public sealed class AndroidMessagesPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidMessagesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new MessagesPageContent(driver.Web(WebViewContext.NhsApp));
        }

        internal AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
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

        public MessagesPageContent PageContent { get; }

        public static AndroidMessagesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidMessagesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }

        public void KeyboardNavigateToTestProvider() =>
            PageContent.KeyboardNavigateToTestProvider(KeyboardPageContentNavigation);

    }
}
