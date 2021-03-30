using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public abstract class AndroidPermissionsDialog
    {

        private readonly IAndroidDriverWrapper _driver;

        protected AndroidPermissionsDialog(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        private AndroidSystemButton AllowButton => AndroidSystemButton.WhichMatches(_driver, "(ALLOW|Allow)");

        private AndroidSystemButton DenyButton => AndroidSystemButton.WhichMatches(_driver, "(DENY|Deny)");

        public void Allow() =>  AllowButton.Click();

        public void Deny() =>  DenyButton.Click();
    }
}