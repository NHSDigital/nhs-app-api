using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSCreateSessionPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Message => new IOSLabel(_driver, "Creating Session");

        internal static IOSCreateSessionPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionPage(driver);
            page.Message.AssertVisible();
            return page;
        }
    }
}