using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSLocationServicesSitePermissionDialog
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSLocationServicesSitePermissionDialog(IIOSDriverWrapper driver, string title)
        {
            _driver = driver;
            _title = title;
        }

        private IOSAlert Alert => IOSAlert.WithMatchingText(_driver, _title);

        public static IOSLocationServicesSitePermissionDialog AssertDisplayed(IIOSDriverWrapper driver)
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            return AssertDisplayed(
                driver,
                $"“https://pkb.securestubs.local.bitraft.io” " +
                "Would Like To Use Your Current Location");
        }

        private static IOSLocationServicesSitePermissionDialog AssertDisplayed(IIOSDriverWrapper driver, string text)
        {
            var dialog = new IOSLocationServicesSitePermissionDialog(driver, text);
            dialog.Alert.AssertText();
            return dialog;
        }

        public void Allow() => Alert.Accept();

        public void Deny() => Alert.Dismiss();
    }
}