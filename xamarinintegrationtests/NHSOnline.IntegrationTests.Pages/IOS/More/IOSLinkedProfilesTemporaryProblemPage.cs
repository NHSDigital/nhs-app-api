using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More
{
    public sealed class IOSLinkedProfilesTemporaryProblemPage
    {
        private IOSFullNavigation Navigation { get; }
        private LinkedProfilesTemporaryProblemPageContent PageContent { get; }

        private IOSLinkedProfilesTemporaryProblemPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new LinkedProfilesTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSLinkedProfilesTemporaryProblemPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLinkedProfilesTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSLinkedProfilesTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}