using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSAppTab
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _headerText;

        private IOSAppTab(IIOSDriverWrapper driver, string headerText = "")
        {
            _driver = driver;
            _headerText = headerText;

        }

        private IOSLabel AppTabHeader => new IOSLabel(_driver, _headerText);
        private IOSButton ReturnToAppButton => new IOSButton(_driver, "Done");

        internal static void AssertAppTabServiceByHeader(IIOSDriverWrapper driver, string headerText)
        {
            var page = new IOSAppTab(driver, headerText);
            page.AppTabHeader.AssertLabelVisible();
            page.ReturnToAppButton.Click();
        }
    }
}