using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSCalendarPermissionErrorDialog
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCalendarPermissionErrorDialog(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSButton GoToSettingsButton => IOSButton.WithText(_driver, "Go to settings");

        private IOSLabel ErrorText => IOSLabel.WhichMatches(_driver,
            "Give NHS App calendar access");

        public static void AssertDisplayed(IIOSDriverWrapper driver)
        {
            var dialog = new IOSCalendarPermissionErrorDialog(driver);
            dialog.ErrorText.AssertVisible();
            dialog.GoToSettingsButton.AssertVisible();
        }
    }
}