using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Messages;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Messages
{
    public class AndroidGpSurgeryMessagesPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidGpSurgeryMessagesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new GpSurgeryMessagesPageContent(driver.Web.NhsAppLoggedInWebView());
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

        private GpSurgeryMessagesPageContent PageContent { get; }

        public static AndroidGpSurgeryMessagesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidGpSurgeryMessagesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void KeyboardNavigateBack() => PageContent.KeyboardNavigateBack(KeyboardPageContentNavigation);
    }
}