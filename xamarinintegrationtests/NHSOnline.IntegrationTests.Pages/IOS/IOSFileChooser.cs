using System;
using System.Threading.Tasks;
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

        private IOSButton PhotosButton => IOSButton.WithText(_driver, "Photos");

        private IOSButton ChooseButton => IOSButton.WithText(_driver, "Choose");

        private IOSSystemImage Photo => IOSSystemImage.WhichMatches(_driver, "Photo,.*");

        public static IOSFileChooser AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSFileChooser(driver);
            page.PhotosButton.AssertVisible();
            return page;
        }

        public IOSFileChooser ChoosePhoto()
        {
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            Photo.Touch();
            return this;
        }

        public void ConfirmSelection()
        {
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            ChooseButton.Touch();
        }
    }
}