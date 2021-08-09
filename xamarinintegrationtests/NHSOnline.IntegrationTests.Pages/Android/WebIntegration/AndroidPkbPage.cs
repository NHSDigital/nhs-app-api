using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidPkbPage
    {
        public AndroidFullNavigation Navigation { get; }
        private PkbPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidPkbPage(IAndroidDriverWrapper driver, string phrPath)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PkbPageContent(driver.Web.WebIntegrationWebView(), phrPath);
        }

        public static AndroidPkbPage AssertOnPage(IAndroidDriverWrapper driver, string phrPath)
        {
            var page = new AndroidPkbPage(driver, phrPath);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidPkbPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllFocusableElements());

        private IEnumerable<IFocusable> GetAllFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public void NavigateToCalendar() => PageContent.NavigateToCalendar();

        public void NavigateToGoToPage() => PageContent.NavigateToGoToPage();

        public void NavigateToFileUpload() => PageContent.NavigateToFileUpload();

        public void NavigateToDocumentDownload() => PageContent.NavigateToDocumentDownload();

        public void NavigateToLocationServices() => PageContent.NavigateToLocationServices();

        public void KeyboardNavigateToPrescriptions() => Navigation.KeyboardNavigateToPrescriptionsFromElement(KeyboardPageContentNavigation, PageContent.FocusableElements.First());

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidPkbPage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }
    }
}