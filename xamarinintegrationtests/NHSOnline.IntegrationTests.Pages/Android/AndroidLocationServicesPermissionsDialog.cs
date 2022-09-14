using System;
using System.Threading;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidLocationServicesPermissionsDialog: AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLocationServicesPermissionsDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidLabel PermissionsText => AndroidLabel.WhichMatches(_driver,
            "Allow .* to access this device.*s location\\?");

        public static AndroidLocationServicesPermissionsDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidLocationServicesPermissionsDialog(driver);
            permissionsDialog.PermissionsText.AssertVisible();
            return permissionsDialog;
        }
    }
}