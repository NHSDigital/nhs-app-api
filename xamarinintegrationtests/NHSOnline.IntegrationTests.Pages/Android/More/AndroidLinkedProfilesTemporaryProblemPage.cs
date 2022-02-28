using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More
{
    public sealed class AndroidLinkedProfilesTemporaryProblemPage
    {
        private AndroidFullNavigation Navigation { get; }
        private LinkedProfilesTemporaryProblemPageContent PageContent { get; }

        private AndroidLinkedProfilesTemporaryProblemPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new LinkedProfilesTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidLinkedProfilesTemporaryProblemPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLinkedProfilesTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidLinkedProfilesTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}