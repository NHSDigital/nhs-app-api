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
            "Select a photo");

        private AndroidLabel FolderText => AndroidLabel.WhichMatches(_driver,
            "Pictures");

        private AndroidPhotoPickerThumbnail ThumbnailPicker => AndroidPhotoPickerThumbnail
            .WithDescription(_driver, "Photo taken on");

        public static AndroidFileChooser AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var permissionsDialog = new AndroidFileChooser(driver);
            permissionsDialog.FileChooserHeaderText.AssertVisible();
            return permissionsDialog;
        }

        public AndroidFileChooser ClickPictures()
        {
            FolderText.Click();
            return this;
        }

        public AndroidFileChooser AssertThumbnailDisplayed()
        {
            ThumbnailPicker.AssertVisible();
            return this;
        }

        public void ChoosePhoto() => ThumbnailPicker.Click();

    }
}