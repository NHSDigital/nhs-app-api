using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidFilePermissionsDialog: AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFilePermissionsDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidLabel PermissionsText => AndroidLabel.WhichMatches(_driver,
            "Allow .* to access .* on your device\\?");

        public static void ClickAllowIfPresent(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidFilePermissionsDialog(driver);
            if (IsPresent(permissionsDialog))
            {
                permissionsDialog.Allow();

            }
        }

        public static void NoActionIfPresent(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidFilePermissionsDialog(driver);
            IsPresent(permissionsDialog);
        }

        public static void ClickDenyIfPresent(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidFilePermissionsDialog(driver);
            if (IsPresent(permissionsDialog))
            {
                permissionsDialog.Deny();

            }
        }

        private static bool IsPresent(AndroidFilePermissionsDialog permissionsDialog)
        {
            return permissionsDialog.PermissionsText.IsPresent();
        }
    }
}