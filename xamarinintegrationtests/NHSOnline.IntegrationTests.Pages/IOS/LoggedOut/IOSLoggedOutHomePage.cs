using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel AccessServicesText => IOSLabel.WithText(_driver, "Access your NHS services");

        private IOSLabel SessionExpiredText => IOSLabel.WithText(_driver, "For your security, you need to log in again");

        private IOSButton ContinueButton => IOSButton.WithText(_driver, "Continue with NHS login");

        private IOSButton HelpIcon => IOSButton.WithText(_driver, "Help");

        private IOSLabel VersionText(string versionNumber) => IOSLabel.WithText(_driver, $"Version {versionNumber}");


        public static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
            page.AccessServicesText.AssertVisible();

            driver.LoggedOutHomeScreenLoaded();

            return page;
        }

        public static void AssertNotOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLoggedOutHomePage(driver);
            page.AccessServicesText.AssertNotPresent();
        }

        public void AssertPageElements()
        {
            AccessServicesText.AssertVisible();
            ContinueButton.AssertVisible();
            HelpIcon.AssertVisible();
        }

        public void AssertHelpIconPresent() => HelpIcon.AssertVisible();

        public void AssertCorrectVersionText() => VersionText(_driver.AppVersionNumber).AssertVisible();

        public void GetHelp() => HelpIcon.Click();

        public void ContinueWithNhsLogin() => ContinueButton.Click();

        public void AssertSessionExpired() => SessionExpiredText.AssertVisible();
    }
}