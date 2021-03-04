using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidSlimCloseNavigation
    {
        private readonly IAndroidDriverWrapper _driver;

        internal AndroidSlimCloseNavigation(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidNavigationHeader Header => AndroidNavigationHeader.WithName(_driver, "NHS App Slim Close Navigation Header");

        private AndroidIcon CloseIcon => Header.ContainingIconWithDescription("NHS App close icon");

        private AndroidLabel CloseText => Header.ContainingLabelWithText("Close");

        public void AssertNavigationPresent()
        {
            Header.AssertVisible();
        }

        public void Close()
        {
            CloseIcon.Click();
        }
    }
}