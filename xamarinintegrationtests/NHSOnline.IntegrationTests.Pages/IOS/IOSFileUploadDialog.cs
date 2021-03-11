using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSFileUploadDialog
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFileUploadDialog(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSLabel BrowseLabel => IOSLabel.WithText(_driver, "Photo Library");

        public static IOSFileUploadDialog AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSFileUploadDialog(driver);
            page.BrowseLabel.AssertVisible();
            return page;
        }

        public void Browse() => BrowseLabel.Click();
    }
}