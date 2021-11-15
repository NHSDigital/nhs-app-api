using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Messages;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Messages
{
    public class AndroidHealthInformationAndUpdatesPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidHealthInformationAndUpdatesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new HealthInformationAndUpdatesPageContent(driver.Web.NhsAppLoggedInWebView());
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

        private HealthInformationAndUpdatesPageContent PageContent { get; }

        public static AndroidHealthInformationAndUpdatesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidHealthInformationAndUpdatesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void KeyboardNavigateBack() => PageContent.KeyboardNavigateBack(KeyboardPageContentNavigation);
    }
}