using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidCamera
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCamera(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        private AndroidGLButton CameraShutterButton => AndroidGLButton.WithText(_driver,"Shutter");

        private AndroidLabel OkLabel => AndroidLabel.WithText(_driver, "OK");

        public static AndroidCamera AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var camera = new AndroidCamera(driver);
            camera.CameraShutterButton.AssertVisible();
            return camera;
        }

        public void TakePhoto() => CameraShutterButton.Click();

        public void ClickOk()
        {
            using var timeout = ExtendedTimeout.FromSeconds(20);
            OkLabel.Click();
        }
    }
}