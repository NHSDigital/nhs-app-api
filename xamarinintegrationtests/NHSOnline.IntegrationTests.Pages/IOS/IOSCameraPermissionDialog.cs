using System;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSCameraPermissionDialog
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSCameraPermissionDialog(IIOSDriverWrapper driver, string title)
        {
            _driver = driver;
            _title = title;
        }

        private IOSAlert Alert => IOSAlert.WithMatchingText(_driver, _title);

        public static IOSCameraPermissionDialog AssertDisplayed(IIOSDriverWrapper driver) => AssertDisplayed(
            driver,
            $"“NHS App BrowserStack” Would Like to Access the Camera{Environment.NewLine}So you can take a photo of your I.D.");

        private static IOSCameraPermissionDialog AssertDisplayed(IIOSDriverWrapper driver, string text)
        {
            var dialog = new IOSCameraPermissionDialog(driver, text);
            dialog.Alert.AssertText();
            return dialog;
        }

        public void Allow()
        {
            Alert.Accept();
        }
    }
}