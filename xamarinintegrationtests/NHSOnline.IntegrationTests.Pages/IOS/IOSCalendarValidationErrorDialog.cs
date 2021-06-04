using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSCalendarValidationErrorDialog
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCalendarValidationErrorDialog(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSButton OkButton => IOSButton.WithText(_driver, "OK");

        private IOSButton AddEventManuallyButton => IOSButton.WithText(_driver, "Add event");

        private IOSLabel ErrorText => IOSLabel.WhichMatches(_driver,
            "Cannot save event");

        public static IOSCalendarValidationErrorDialog AssertDisplayed(IIOSDriverWrapper driver)
        {
            var dialog = new IOSCalendarValidationErrorDialog(driver);
            dialog.ErrorText.AssertVisible();
            return dialog;
        }

        public void Ok() =>  OkButton.Click();

        public void AddEventManually() =>  AddEventManuallyButton.Click();
    }
}