using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSLoggedOutHomePage
    {
        private readonly IIOSDriverWrapper _iosDriver;

        private IOSLoggedOutHomePage(IIOSDriverWrapper iosDriver) => _iosDriver = iosDriver;

        private IOSLabel WelcomeMessage => new IOSLabel(_iosDriver, "Welcome to the NHS App");

        internal static IOSLoggedOutHomePage AssertOnPage(IIOSDriverWrapper iosDriver)
        {
            var page = new IOSLoggedOutHomePage(iosDriver);
            page.WelcomeMessage.AssertVisible();
            return page;
        }
    }
}