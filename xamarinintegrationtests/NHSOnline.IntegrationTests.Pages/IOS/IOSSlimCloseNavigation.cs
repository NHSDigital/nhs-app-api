using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSSlimCloseNavigation
    {
        private readonly IIOSDriverWrapper _driver;

        internal IOSSlimCloseNavigation(IIOSDriverWrapper driver) => _driver = driver;

        private IOSButton CloseIcon => IOSButton.WithText(_driver, "Close");

        public void Close()
        {
            CloseIcon.Click();
        }
    }
}