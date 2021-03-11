using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSFileChooser
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFileChooser(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSSystemLinkLabel AllPhotos => IOSSystemLinkLabel.WithText(_driver, "All Photos");

        private IOSLabel FileChooserTitleText => IOSLabel.WithText(_driver, "Photos");

        private IOSButton Done => IOSButton.WithText(_driver, "Done");

        private IOSSystemLinkLabel Photo => IOSSystemLinkLabel.WhichMatches(_driver, "Photo,.*");

        public static IOSFileChooser AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSFileChooser(driver);
            page.FileChooserTitleText.AssertVisible();
            return page;
        }

        public IOSFileChooser SelectFolder()
        {
            AllPhotos.Click();
            return this;
        }

        public IOSFileChooser ChoosePhoto()
        {
            Photo.Click();
            return this;
        }

        public IOSFileChooser ConfirmSelection()
        {
            Done.Click();
            return this;
        }
    }
}