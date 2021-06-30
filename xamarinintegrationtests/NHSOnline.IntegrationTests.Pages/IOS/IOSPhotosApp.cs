using System;
using FluentAssertions.Execution;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public sealed class IOSPhotosApp
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSButton PhotosButton => IOSButton.WithText(_driver, "Photos");

        private IOSPhotoCell Photo => IOSPhotoCell.WithName(_driver, DateTime.UtcNow);

        private IOSPhotoCell PhotoFallback => IOSPhotoCell.WithName(_driver, DateTime.UtcNow.AddMinutes(-1));

        private IOSPhotosApp(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        public static IOSPhotosApp AssertDisplayed(IIOSDriverWrapper driver)
        {
            var screen = new IOSPhotosApp(driver);
            screen.PhotosButton.AssertVisible();
            return screen;
        }

        public IOSPhotosApp SelectPhotosTab()
        {
            PhotosButton.Click();
            return this;
        }

        public void AssertPhotoVisible()
        {
            using var timeout = ExtendedTimeout.FromSeconds(5);

            try
            {
                Photo.AssertVisible();
            }
            catch (AssertionFailedException)
            {
                PhotoFallback.AssertVisible();
            }
        }
    }
}