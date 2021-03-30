using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

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

        public static AndroidFilePermissionsDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidFilePermissionsDialog(driver);
            permissionsDialog.PermissionsText.AssertVisible();
            return permissionsDialog;
        }
    }
}