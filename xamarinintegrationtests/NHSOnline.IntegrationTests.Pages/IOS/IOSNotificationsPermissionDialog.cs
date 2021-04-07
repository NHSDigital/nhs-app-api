using System;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSNotificationsPermissionDialog
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _match;

        private IOSNotificationsPermissionDialog(IIOSDriverWrapper driver, string match)
        {
            _driver = driver;
            _match = match;
        }

        private IOSAlert Alert => IOSAlert.WithMatchingText(_driver, _match);

        public static void AssertDisplayed(IIOSDriverWrapper driver)
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            AssertDisplayed(
                driver,
                $"“NHS App BrowserStack” Would Like to Send You Notifications{Environment.NewLine}" +
                "Notifications may include alerts, sounds,? and icon badges. These can be configured in Settings.");
        }

        private static void AssertDisplayed(IIOSDriverWrapper driver, string text)
        {
            var dialog = new IOSNotificationsPermissionDialog(driver, text);
            dialog.Alert.AssertTextMatches();
        }
    }
}