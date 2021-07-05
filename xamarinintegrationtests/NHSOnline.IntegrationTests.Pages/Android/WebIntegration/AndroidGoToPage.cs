using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidGoToPage
    {
        private AndroidFullNavigation Navigation { get; }
        private GoToPagePageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidGoToPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new GoToPagePageContent(driver.Web(WebViewContext.TestProviderWebIntegration));
        }

        public static AndroidGoToPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidGoToPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidGoToPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, PageContent.FocusableElements);

        public void KeyboardNavigateToGoToHomePage() => PageContent.KeyboardNavigateToGoToHome(KeyboardPageContentNavigation);

        public void KeyboardNavigateToGoToPrescriptions() => PageContent.KeyboardNavigateToGoToPrescriptions(KeyboardPageContentNavigation);

        public void KeyboardNavigateToGoToMessages() => PageContent.KeyboardNavigateToGoToMessages(KeyboardPageContentNavigation);

        public void KeyboardNavigateToGoToInvalidPage() => PageContent.KeyboardNavigateToGoToInvalidPage(KeyboardPageContentNavigation);


        public void KeyboardNavigateToGoToAppointments() => PageContent.KeyboardNavigateToGoToAppointments(KeyboardPageContentNavigation);

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidGoToPage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }
    }
}