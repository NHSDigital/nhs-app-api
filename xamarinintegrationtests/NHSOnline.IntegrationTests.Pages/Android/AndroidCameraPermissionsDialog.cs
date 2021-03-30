using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidCameraPermissionsDialog: AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCameraPermissionsDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidLabel PermissionsText => AndroidLabel.WhichMatches(_driver,
            "Allow .* to take pictures and record video\\?");

        public static AndroidCameraPermissionsDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidCameraPermissionsDialog(driver);
            permissionsDialog.PermissionsText.AssertVisible();
            return permissionsDialog;
        }
    }
}