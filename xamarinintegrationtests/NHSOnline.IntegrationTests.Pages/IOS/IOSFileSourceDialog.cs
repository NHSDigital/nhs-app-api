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

        private IOSButton PhotoLibraryLabel => IOSButton.WithText(_driver, "Photo Library");

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
            if (page.BrowseLabel.IsPresent())
            {
                action(page);
            }
        }

        public void SelectPhotoLibrary() => PhotoLibraryLabel.Touch();

        public void SelectBrowse() => BrowseLabel.Click();

        public void TakePhoto() => TakePhotoLabel.Touch();
    }
}