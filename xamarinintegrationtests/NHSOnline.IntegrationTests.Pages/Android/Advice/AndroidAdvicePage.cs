using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Advice
{
    public sealed class AndroidAdvicePage
    {
        private readonly IAndroidDriverWrapper _driver;

        internal AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            GetAllKeyboardHomeNavigationFocusableElements());

        private AndroidAdvicePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AdvicePageContent(driver.Web(WebViewContext.NhsApp));
        }

        private IEnumerable<IFocusable> GetAllKeyboardHomeNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public AndroidFullNavigation Navigation { get; }

        private AdvicePageContent PageContent { get; }

        public static AndroidAdvicePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAdvicePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }

        public void KeyboardNavigateToHome() =>
            Navigation.KeyboardNavigateToHome(KeyboardPageContentNavigation);
    }
}
