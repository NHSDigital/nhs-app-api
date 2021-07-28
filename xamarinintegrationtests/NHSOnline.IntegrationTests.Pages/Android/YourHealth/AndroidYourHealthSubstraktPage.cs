using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public class AndroidYourHealthSubstraktPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidYourHealthSubstraktPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new YourHealthSubstraktPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public YourHealthSubstraktPageContent PageContent { get; }

        public static AndroidYourHealthSubstraktPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthSubstraktPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        private IEnumerable<IFocusable> GetAllKeyboardNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardNavigationFocusableElements());

        public void KeyboardNavigateToSubstrakt() =>
            PageContent.KeyboardNavigateToSubstrakt(KeyboardPageContentNavigation);
    }
}