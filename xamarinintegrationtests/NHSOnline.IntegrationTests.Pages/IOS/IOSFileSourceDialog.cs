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

        private IOSButton BrowseLabel => IOSButton.WithText(_driver, "Choose File");

        private IOSSystemLabel TakePhotoLabel => IOSSystemLabel.WithText(_driver, "Take Photo");

        public static IOSFileSourceDialog GetPanel(IIOSDriverWrapper driver)
        {
            var page = new IOSFileSourceDialog(driver);
            return page;
        }

        public static void IfDisplayed(IIOSDriverWrapper driver, Action<IOSFileSourceDialog> action)
        {
            var page = new IOSFileSourceDialog(driver);
            if (page.BrowseLabel.IsVisible())
            {
                action(page);
            }
        }

        public void SelectPhotoLibrary() => PhotoLibraryLabel.Touch();

        public void TakePhoto() => TakePhotoLabel.Touch();
    }
}