using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidLocationProminentDialog: AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLocationProminentDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidLabel DialogTitle => AndroidLabel.WithText(_driver, "Location access");

        private AndroidButton OkButton => AndroidButton.WithText(_driver, "Allow");

        public static AndroidLocationProminentDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidLocationProminentDialog(driver);
            permissionsDialog.DialogTitle.AssertVisible();
            return permissionsDialog;
        }

        public void Ok() => OkButton.Click();
    }
}