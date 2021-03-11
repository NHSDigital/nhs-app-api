using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidPermissionsDialog(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        private AndroidLabel PermissionsText => AndroidLabel.WhichMatches(_driver,
            "Allow .* to access .* on your device\\?");

        private AndroidSystemButton AllowButton => AndroidSystemButton.WhichMatches(_driver, "(ALLOW|Allow)");

        private AndroidSystemButton DenyButton => AndroidSystemButton.WhichMatches(_driver, "(DENY|Deny)");

        public static AndroidPermissionsDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidPermissionsDialog(driver);
            permissionsDialog.PermissionsText.AssertVisible();
            return permissionsDialog;
        }

        public void Allow() => AllowButton.Click();

        public void Deny() => DenyButton.Click();
    }
}