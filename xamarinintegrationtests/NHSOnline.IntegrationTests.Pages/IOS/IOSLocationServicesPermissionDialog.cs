using System;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSLocationServicesPermissionDialog
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSLocationServicesPermissionDialog(IIOSDriverWrapper driver, string title)
        {
            _driver = driver;
            _title = title;
        }

        private IOSMultiPermissionAlert Alert => IOSMultiPermissionAlert.WithMatchingText(_driver, _title);

        public static IOSLocationServicesPermissionDialog AssertDisplayed(IIOSDriverWrapper driver)
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            return AssertDisplayed(
                driver,
                $"Allow “NHS App BrowserStack” to access your location?{Environment.NewLine}" +
                "We need to know where you are so we can find services near you to help.");
        }

        private static IOSLocationServicesPermissionDialog AssertDisplayed(IIOSDriverWrapper driver, string text)
        {
            var dialog = new IOSLocationServicesPermissionDialog(driver, text);
            dialog.Alert.AssertText();
            return dialog;
        }

        public void Allow() => Alert.AcceptAllowOnce();

        public void Deny() => Alert.DoNotAllow();
    }
}