using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidSettingsAppInfo
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidSettingsAppInfo(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "App info");

        private AndroidLabel Notifications => AndroidLabel.WithText(_driver, "Notifications");

        public static AndroidSettingsAppInfo AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidSettingsAppInfo(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void NavigateToNotifications() => Notifications.Click();
    }
}