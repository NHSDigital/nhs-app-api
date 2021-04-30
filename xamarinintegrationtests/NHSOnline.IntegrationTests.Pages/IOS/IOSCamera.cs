using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSCamera
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCamera(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        public static IOSCamera AssertDisplayed(IIOSDriverWrapper driver)
        {
            var camera = new IOSCamera(driver);
            camera.IOSCameraButton.AssertVisible();
            return camera;
        }

        private IOSButton IOSCameraButton => IOSButton.WithText(_driver, "Take Picture");

        private IOSButton UsePhotoButton => IOSButton.WithText(_driver, "Use Photo");

        public IOSCamera Capture()
        {
            IOSCameraButton.Click();
            return this;
        }

        public void UsePhoto() => UsePhotoButton.Click();
    }
}