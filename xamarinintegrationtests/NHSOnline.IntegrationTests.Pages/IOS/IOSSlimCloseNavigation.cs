using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSSlimCloseNavigation
    {
        private readonly IIOSDriverWrapper _driver;

        internal IOSSlimCloseNavigation(IIOSDriverWrapper driver) => _driver = driver;

        private IOSNavigationHeader Header => IOSNavigationHeader.WithName(_driver, "NHS App Slim Close Navigation Header");

        private IOSIcon CloseIcon => Header.ContainingIconWithDescription("NHS App close icon");

        private IOSLabel CloseText => Header.ContainingLabelWithText("Close");

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