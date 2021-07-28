using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidWebIntegrationWarningPanelPage
    {
        private AndroidFullNavigation Navigation { get; }
        public WebIntegrationWarningPanelPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidWebIntegrationWarningPanelPage(IAndroidDriverWrapper driver, string pageTitle)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new WebIntegrationWarningPanelPageContent(driver.Web.NhsAppLoggedInWebView(), pageTitle);
        }

        public static AndroidWebIntegrationWarningPanelPage AssertOnPage(IAndroidDriverWrapper driver, string pageTitle)
        {
            var page = new AndroidWebIntegrationWarningPanelPage(driver, pageTitle);
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

        public void KeyboardNavigateToContinue() => PageContent.KeyboardNavigateContinue(KeyboardPageContentNavigation);

        public void KeyboardNavigateBack() => PageContent.KeyboardNavigateBack(KeyboardPageContentNavigation);
    }
}