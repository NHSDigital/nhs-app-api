using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public sealed class IOSYourHealthTemporaryProblemPage
    {
        private IOSFullNavigation Navigation { get; }
        private YourHealthTemporaryProblemPageContent PageContent { get; }

        private IOSYourHealthTemporaryProblemPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new YourHealthTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSYourHealthTemporaryProblemPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSYourHealthTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSYourHealthTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}