using System;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSCalendarPermissionDialog
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSCalendarPermissionDialog(IIOSDriverWrapper driver, string title)
        {
            _driver = driver;
            _title = title;
        }

        private IOSAlert Alert => IOSAlert.WithMatchingText(_driver, _title);

        public static IOSCalendarPermissionDialog AssertDisplayed(IIOSDriverWrapper driver) => AssertDisplayed(
            driver,
            $"“NHS App BrowserStack” Would Like to Access Your Calendar{Environment.NewLine}So we can add events to your calendar, like appointments.");

        private static IOSCalendarPermissionDialog AssertDisplayed(IIOSDriverWrapper driver, string text)
        {
            var dialog = new IOSCalendarPermissionDialog(driver, text);
            dialog.Alert.AssertText();
            return dialog;
        }

        public void Allow() => Alert.Accept();

        public void Deny() => Alert.Dismiss();
    }
}