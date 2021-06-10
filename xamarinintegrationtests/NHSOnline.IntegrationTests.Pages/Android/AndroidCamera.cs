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

        private AndroidImageButton CameraShutterButton => AndroidImageButton.WithDescription(_driver,
            "Take photo", "Shutter");

        private AndroidImageButton DoneButton => AndroidImageButton.WithDescription(_driver,
            "Done");

        public static AndroidCamera AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var camera = new AndroidCamera(driver);
            camera.CameraShutterButton.AssertEitherVisible();
            return camera;
        }

        public AndroidCamera TakePhoto()
        {
            CameraShutterButton.ClickEither();
            return this;
        }

        public void Done()
        {
            using var timeout = ExtendedTimeout.FromSeconds(20);
            DoneButton.Click();
        }
    }
}