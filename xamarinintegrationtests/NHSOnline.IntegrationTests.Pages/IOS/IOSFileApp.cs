using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSFilesApp
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFilesApp(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSFilesAppHeader FileName => IOSFilesAppHeader.WithText(_driver, "myImage");

        private IOSDocumentInteractionControllerOption SaveImageOption => IOSDocumentInteractionControllerOption.WithText(_driver, "Save Image");

        public static IOSFilesApp AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSFilesApp(driver);
            page.FileName.AssertVisible();
            return page;
        }

        public void SelectSaveImage() => SaveImageOption.Click();
    }
}