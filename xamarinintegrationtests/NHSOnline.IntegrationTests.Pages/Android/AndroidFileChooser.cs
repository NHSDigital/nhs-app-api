using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidFileChooser
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFileChooser(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        private AndroidLabel FileChooserHeaderText => AndroidLabel.WhichMatches(_driver,
            "Recent");

        private AndroidImageButton HeaderMenuButton => AndroidImageButton.WithDescription(_driver, "Show roots");

        private AndroidLabel ImagesMenuLabel => AndroidLabel.WhichMatches(_driver,
            "Images");

        private AndroidLabel PicturesLabel => AndroidLabel.WhichMatches(_driver,
            "Pictures");

        private AndroidLabel BrowserStackThumbnailLabel => AndroidLabel.WhichMatches(_driver,
            "BrowserStack.jpg");

        public static AndroidFileChooser AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidFileChooser(driver);
            permissionsDialog.FileChooserHeaderText.AssertVisible();
            return permissionsDialog;
        }

        public AndroidFileChooser HamburgerClick()
        {
            HeaderMenuButton.Click();
            return this;
        }

        public AndroidFileChooser ImagesMenuLabelClick()
        {
            ImagesMenuLabel.Click();
            return this;
        }

        public AndroidFileChooser PicturesLabelClick()
        {
            PicturesLabel.Click();
            return this;
        }

        public AndroidFileChooser AssertThumbnailDisplayed()
        {
            BrowserStackThumbnailLabel.AssertVisible();
            return this;
        }

        public void ChoosePhoto() => BrowserStackThumbnailLabel.Click();
    }
}