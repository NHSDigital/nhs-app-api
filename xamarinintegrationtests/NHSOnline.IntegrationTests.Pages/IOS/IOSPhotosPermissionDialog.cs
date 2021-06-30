using System;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSPhotosPermissionDialog
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSPhotosPermissionDialog(IIOSDriverWrapper driver, string title)
        {
            _driver = driver;
            _title = title;
        }

        private IOSAlert Alert => IOSAlert.WithMatchingText(_driver, _title);

        public static IOSPhotosPermissionDialog AssertDisplayed(IIOSDriverWrapper driver) => AssertDisplayed(
            driver,
            $"“NHS App BrowserStack” Would Like to Add to your Photos{Environment.NewLine}This is so that you can download your photos.");

        private static IOSPhotosPermissionDialog AssertDisplayed(IIOSDriverWrapper driver, string text)
        {
            var dialog = new IOSPhotosPermissionDialog(driver, text);
            dialog.Alert.AssertText();
            return dialog;
        }

        public void Allow() => Alert.Accept();

        public void Deny() => Alert.Dismiss();
    }
}