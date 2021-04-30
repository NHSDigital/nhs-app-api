using System;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSImageSourceDialog
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSImageSourceDialog(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSSystemLabel BrowseLabel => IOSSystemLabel.WithText(_driver, "Photo Library");

        private IOSSystemLabel TakePhotoLabel => IOSSystemLabel.WithText(_driver, "Take Photo");

        public static IOSImageSourceDialog AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSImageSourceDialog(driver);
            page.BrowseLabel.AssertVisible();
            return page;
        }

        public static void IfDisplayed(IIOSDriverWrapper driver, Action<IOSImageSourceDialog> action)
        {
            var page = new IOSImageSourceDialog(driver);
            if (page.BrowseLabel.IsPresent())
            {
                action(page);
            }
        }

        public void Browse() => BrowseLabel.Click();

        public void TakePhoto() => TakePhotoLabel.Click();
    }
}