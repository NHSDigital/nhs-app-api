using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidCalendarErrorDialog: AndroidPermissionsDialog
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCalendarErrorDialog(IAndroidDriverWrapper driver): base(driver)
        {
            _driver = driver;
        }

        private AndroidSystemButton OkButton => AndroidSystemButton.WhichMatches(_driver, "OK");

        private AndroidSystemButton AddEventManuallyButton => AndroidSystemButton.WhichMatches(_driver, "Add event");

        private AndroidLabel ErrorText => AndroidLabel.WhichMatches(_driver,
            "Cannot save event");

        public static AndroidCalendarErrorDialog AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var dialog = new AndroidCalendarErrorDialog(driver);
            dialog.ErrorText.AssertVisible();
            return dialog;
        }


        public void Ok() =>  OkButton.Click();

        public void AddEventManually() =>  AddEventManuallyButton.Click();
    }
}