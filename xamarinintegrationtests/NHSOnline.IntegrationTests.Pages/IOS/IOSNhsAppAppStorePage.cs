using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSNhsAppAppStorePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSNhsAppAppStorePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel ApplicationElement => IOSLabel.WithText(_driver, "Cannot Connect to App Store");

        private IOSButton RetryButton => IOSButton.WithText(_driver, "Retry");

        public static IOSNhsAppAppStorePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNhsAppAppStorePage(driver);            
            page.RetryButton.AssertVisible();
            return page;
        }
    }
}