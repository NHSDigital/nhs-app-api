using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.YourHealth
{
    public sealed class AndroidYourHealthTemporaryProblemPage
    {
        private AndroidFullNavigation Navigation { get; }
        private YourHealthTemporaryProblemPageContent PageContent { get; }

        private AndroidYourHealthTemporaryProblemPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new YourHealthTemporaryProblemPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidYourHealthTemporaryProblemPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidYourHealthTemporaryProblemPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidYourHealthTemporaryProblemPage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void TryAgain() => PageContent.TryAgain();
    }
}