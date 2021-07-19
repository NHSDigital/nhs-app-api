using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSPassKitController
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSPassKitController(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSLabel PassKitText => IOSLabel.WithText(_driver, "Company Staff ID");

        public static void AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSPassKitController(driver);
            page.PassKitText.AssertVisible();
        }
    }
}