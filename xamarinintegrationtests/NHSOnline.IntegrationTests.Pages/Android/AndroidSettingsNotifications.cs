using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidSettingsNotifications
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidSettingsNotifications(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidImageButton Back => AndroidImageButton.WithDescription(_driver, "Navigate up");

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "App notifications");

        private AndroidToggle NotificationsEnabled => AndroidToggle.WithText(_driver, "On");

        public static AndroidSettingsNotifications AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidSettingsNotifications(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidSettingsNotifications AssertPageContent()
        {
            NotificationsEnabled.AssertVisible();
            return this;
        }

        public void NavigateBack() => Back.Click();
    }
}