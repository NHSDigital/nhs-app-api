using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSHomeScreen
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSIcon Photos => IOSIcon.WithName(_driver, "Photos");

        private IOSHomeScreen(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        public static IOSHomeScreen AssertDisplayed(IIOSDriverWrapper driver)
        {
            var screen = new IOSHomeScreen(driver);
            screen.Photos.AssertVisible();
            return screen;
        }

        public void SelectPhotosApplication() => Photos.Click();
    }
}