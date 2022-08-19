using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidStorageProminentDialog: AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidStorageProminentDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidLabel DialogTitle => AndroidLabel.WithText(_driver, "File storage access");

        private AndroidButton AllowButton => AndroidButton.WithText(_driver, "Allow");

        private AndroidButton CancelButton => AndroidButton.WithText(_driver, "Cancel");

        public static AndroidStorageProminentDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidStorageProminentDialog(driver);
            permissionsDialog.DialogTitle.AssertVisible();
            return permissionsDialog;
        }

        public void Ok() => AllowButton.Click();

        public void Cancel() => CancelButton.Click();
    }
}