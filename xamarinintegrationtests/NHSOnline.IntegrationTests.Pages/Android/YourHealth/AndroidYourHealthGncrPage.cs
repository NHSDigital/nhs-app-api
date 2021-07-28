using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public class AndroidYourHealthGncrPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidYourHealthGncrPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new YourHealthGncrPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public YourHealthGncrPageContent PageContent { get; }

        public static AndroidYourHealthGncrPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthGncrPage(driver);
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

        public void KeyboardNavigateToGncr() =>
            PageContent.KeyboardNavigateToGncr(KeyboardPageContentNavigation);
    }
}