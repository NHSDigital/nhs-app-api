using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidCameraProminentDialog: AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCameraProminentDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidLabel DialogTitle => AndroidLabel.WithText(_driver, "Camera access");

        private AndroidButton OkButton => AndroidButton.WithText(_driver, "Allow");

        public static AndroidCameraProminentDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidCameraProminentDialog(driver);
            permissionsDialog.DialogTitle.AssertVisible();
            return permissionsDialog;
        }

        public void Ok() => OkButton.Click();
    }
}