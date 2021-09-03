using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.Biometrics;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.Biometrics
{
    public class IOSTouchIdRegistrationPage
    {
        private IOSTouchIdRegistrationPage(IIOSDriverWrapper driver)
        {
            var webInteractor = driver.Web.NhsAppLoggedInWebView();

            Navigation = new IOSFullNavigation(driver);
            PageContent = new TouchIdRegistrationPageContent(webInteractor);
        }

        public TouchIdRegistrationPageContent PageContent { get; }

        public IOSFullNavigation Navigation { get; }

        public static IOSTouchIdRegistrationPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSTouchIdRegistrationPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }
    }
}