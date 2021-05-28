using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidAToZPage
    {
        private AndroidFullNavigation Navigation { get; }
        private AToZPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAToZPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AToZPageContent(driver.Web(WebViewContext.AToZWebIntegration));
        }

        public static AndroidAToZPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAToZPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllFocusableElements());

        private IEnumerable<IFocusable> GetAllFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();

            return footerFocusableList.Concat(headerFocusableList);
        }

        public void KeyboardNavigateToMessages() => Navigation.KeyboardNavigateToMessages(KeyboardPageContentNavigation);
    }
}