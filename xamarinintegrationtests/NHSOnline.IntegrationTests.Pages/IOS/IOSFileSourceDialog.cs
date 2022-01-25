using System;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSFileSourceDialog
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFileSourceDialog(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSButton PhotoLibraryButton => IOSButton.WithText(_driver, "Photo Library");

        private IOSButton BrowseButton => IOSButton.WithText(_driver, "Browse");

        private IOSSystemLabel BrowseLabel => IOSSystemLabel.WithText(_driver, "Browse");

        private IOSSystemLabel TakePhotoLabel => IOSSystemLabel.WithText(_driver, "Take Photo");

        public static IOSFileSourceDialog GetPanel(IIOSDriverWrapper driver)
        {
            var page = new IOSFileSourceDialog(driver);
            return page;
        }

        public static void IfDisplayed(IIOSDriverWrapper driver, Action<IOSFileSourceDialog> action)
        {
            var page = new IOSFileSourceDialog(driver);
            if (page.BrowseButton.IsVisible())
            {
                action(page);
            }
        }

        public void SelectPhotoLibrary() => PhotoLibraryButton.Click();

        public void SelectBrowse() => BrowseLabel.Click();

        public void TakePhoto() => TakePhotoLabel.Click();
    }
}