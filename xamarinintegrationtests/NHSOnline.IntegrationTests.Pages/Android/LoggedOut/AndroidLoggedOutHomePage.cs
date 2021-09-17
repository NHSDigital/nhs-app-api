using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidLoggedOutHomePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLoggedOutHomePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel AccessServicesText => AndroidLabel.WithText(_driver, "Access your NHS services");

        private AndroidLabel SessionExpiredText => AndroidLabel.WithText(_driver, "For your security, you need to log in again");

        private AndroidNhsLoginButton ContinueButton => AndroidNhsLoginButton.WithContentDescription(_driver, "Continue with NHS login");

        private AndroidIcon HelpIcon => AndroidIcon.WithName(_driver, "NHS App help icon");

        private AndroidLabel VersionText(string versionNumber) => AndroidLabel.WithText(_driver, $"Version {versionNumber}");

        public static AndroidLoggedOutHomePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidLoggedOutHomePage(driver);
            page.AccessServicesText.AssertVisible();

            driver.NhsAppWebViewClosed();

            return page;
        }

        public void AssertPageElements()
        {
            AccessServicesText.AssertVisible();
            ContinueButton.AssertVisible();
            HelpIcon.AssertVisible();
        }

        public void GetHelp() => HelpIcon.Click();

        public void ContinueWithNhsLogin() => ContinueButton.Click();

        public void AssertSessionExpired() => SessionExpiredText.AssertVisible();

        public void AssertCorrectVersionText() => VersionText(_driver.AppVersionNumber).AssertVisible();
    }
}
