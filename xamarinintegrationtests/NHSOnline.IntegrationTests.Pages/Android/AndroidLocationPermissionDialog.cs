using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidLocationPermissionDialog : AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLocationPermissionDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidLabel PermissionsText => AndroidLabel.WhichMatches(_driver,
            "Allow .* to access this device's location\\?");

        public static AndroidPermissionsDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidLocationPermissionDialog(driver);
            permissionsDialog.PermissionsText.AssertVisible();
            return permissionsDialog;
        }
    }
}