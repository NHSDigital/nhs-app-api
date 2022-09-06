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

        private IOSFilesAppHeader ImageFileDetailsHeader => IOSFilesAppHeader.WithText(_driver, "MyHandAndFootXrayPicture, Image · 8 KB");
        private IOSFilesAppHeader ZipFileDetailsHeader => IOSFilesAppHeader.WithText(_driver, "TestZipFile, ZIP Archive · 82 KB");
        private IOSFilesAppHeader DocumentFileDetailsHeader => IOSFilesAppHeader.WithText(_driver, "06_08_2022, PDF Document · 89 KB");

        public static IOSShareFilePanel AssertDisplayedImageFile(IIOSDriverWrapper driver)
        {
            var page = new IOSShareFilePanel(driver);
            page.ImageFileDetailsHeader.AssertVisible();
            return page;
        }

        public static IOSShareFilePanel AssertDisplayedZipFile(IIOSDriverWrapper driver)
        {
            var page = new IOSShareFilePanel(driver);
            page.ZipFileDetailsHeader.AssertVisible();
            return page;
        }

        public static IOSShareFilePanel AssertDisplayedDocumentFile(IIOSDriverWrapper driver)
        {
            var page = new IOSShareFilePanel(driver);
            page.DocumentFileDetailsHeader.AssertVisible();
            return page;
        }
    }
}