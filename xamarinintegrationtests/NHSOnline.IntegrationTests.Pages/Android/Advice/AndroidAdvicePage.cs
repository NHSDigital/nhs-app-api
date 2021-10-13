using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Advice;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Advice
{
    public sealed class AndroidAdvicePage
    {
        public AndroidFullNavigation Navigation { get; }
        public AdvicePageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAdvicePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AdvicePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAdvicePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAdvicePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();
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

        public void NavigateToAppointments() => Navigation.NavigateToAppointments();

        public void KeyboardNavigateToHome() => Navigation.KeyboardNavigateToHomeFromElement(KeyboardPageContentNavigation, PageContent.FocusableElements.First());
        public void KeyboardNavigateToOneOneOne() => PageContent.KeyboardNavigateToOneOneOne(KeyboardPageContentNavigation);
        public void KeyboardNavigateToAToZ() => PageContent.KeyboardNavigateToAToZ(KeyboardPageContentNavigation);
    }
}
