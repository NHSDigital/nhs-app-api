using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSShareFilePanel
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSShareFilePanel(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSFilesAppHeader FileDetailsHeader => IOSFilesAppHeader.WithText(_driver, "MyHandAndFootXrayPicture, Image · 8 KB");

        public static IOSShareFilePanel AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSShareFilePanel(driver);
            page.FileDetailsHeader.AssertVisible();
            return page;
        }
    }
}