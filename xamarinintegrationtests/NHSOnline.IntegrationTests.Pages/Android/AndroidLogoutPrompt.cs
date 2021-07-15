using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidLogoutPrompt
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLogoutPrompt(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        private AndroidLabel LogoutText => AndroidLabel.WithText(_driver,
            "Are you sure you want to log out?");

        private AndroidSystemButton LogoutButton => AndroidSystemButton.WhichMatches(_driver, "Log out");

        private AndroidSystemButton CancelButton => AndroidSystemButton.WhichMatches(_driver, "Cancel");

        public static AndroidLogoutPrompt AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var logoutPrompt = new AndroidLogoutPrompt(driver);
            logoutPrompt.LogoutText.AssertVisible();
            return logoutPrompt;
        }

        public void Logout() => LogoutButton.Click();

        public void Cancel() => CancelButton.Click();

    }
}