using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidOneOneOnePage
    {
        private AndroidFullNavigation Navigation { get; }
        private OneOneOnePageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidOneOneOnePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new OneOneOnePageContent(driver.Web(WebViewContext.OneOneOneWebIntegration));
        }

        public static AndroidOneOneOnePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidOneOneOnePage(driver);
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

        public void KeyboardNavigateToAppointments() => Navigation.KeyboardNavigateToAppointments(KeyboardPageContentNavigation);
    }
}