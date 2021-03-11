using System;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSPermissionsDialog
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSPermissionsDialog(IIOSDriverWrapper driver, string title)
        {
            _driver = driver;
            _title = title;
        }

        private IOSAlert Alert => IOSAlert.WithText(_driver, _title);

        public static IOSPermissionsDialog AssertDisplayedForCamera(IIOSDriverWrapper driver)
            => AssertDisplayed(driver, $"“NHSOnline.App” Would Like to Access the Camera{Environment.NewLine}So you can take a photo of your I.D.");

        private static IOSPermissionsDialog AssertDisplayed(IIOSDriverWrapper driver, string title)
        {
            var permissionsDialog = new IOSPermissionsDialog(driver, title);
            permissionsDialog.Alert.AssertVisible();
            return permissionsDialog;
        }

        public void Okay()
        {
            Alert.Accept();
        }
    }
}