using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSBeforeYouStartPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSBeforeYouStartPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => new IOSLabel(_driver, "Before You Start");

        internal static IOSBeforeYouStartPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSBeforeYouStartPage(driver);
            page.Title.AssertVisible();
            return page;
        }
    }
}