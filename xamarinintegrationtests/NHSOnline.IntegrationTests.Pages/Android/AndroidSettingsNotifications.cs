using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidSettingsNotifications
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidSettingsNotifications(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidImageButton Back => AndroidImageButton.WithDescription(_driver, "Navigate up");

        private AndroidToggle NotificationsEnabled => AndroidToggle.WithTextMatches(_driver, "(?i)Notifications, Show notifications.*");

        public static AndroidSettingsNotifications  AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidSettingsNotifications(driver);
            page.NotificationsEnabled.AssertVisible();
            return page;
        }

        public void TurnOffNotifications() => NotificationsEnabled.Click();

        public void ClickBack() => Back.Click();

        public void AssertPageContent()
        {
            NotificationsEnabled.AssertVisible();
        }
    }
}