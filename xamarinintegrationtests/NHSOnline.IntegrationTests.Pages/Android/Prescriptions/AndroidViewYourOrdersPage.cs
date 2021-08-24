using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public class AndroidViewYourOrdersPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFullNavigation Navigation { get; }

        private ViewYourOrdersPageContent PageContent { get; }

        private AndroidViewYourOrdersPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new ViewYourOrdersPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidViewYourOrdersPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidViewYourOrdersPage(driver);
            page.PageContent.AssertOnPage();
            return page;
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

        public void KeyboardNavigateBack() =>
            PageContent.KeyboardNavigateBack(KeyboardPageContentNavigation);
    }
}