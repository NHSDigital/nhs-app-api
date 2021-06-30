using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidGooglePhotosApp
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidGooglePhotosApp(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidPhotosAppImage OpenedImage =>
            AndroidPhotosAppImage.WithImageViewDescription(_driver, "Photo taken on ");

        public static AndroidGooglePhotosApp AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidGooglePhotosApp(driver);
            page.OpenedImage.AssertVisible();
            return page;
        }
    }
}