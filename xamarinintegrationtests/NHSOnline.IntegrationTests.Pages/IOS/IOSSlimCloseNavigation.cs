using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSSlimCloseNavigation
    {
        private readonly IIOSDriverWrapper _driver;

        internal IOSSlimCloseNavigation(IIOSDriverWrapper driver) => _driver = driver;

        private IOSNavigationBar Header => IOSNavigationBar.WithName(_driver, "NHS App Slim Close Navigation Header");

        private IOSAppIcon CloseIcon => Header.ContainingButtonWithName("Close");

        internal void AssertNavigationPresent()
        {
            Header.AssertVisible();
        }

        public void Close()
        {
            CloseIcon.Click();
        }
    }
}