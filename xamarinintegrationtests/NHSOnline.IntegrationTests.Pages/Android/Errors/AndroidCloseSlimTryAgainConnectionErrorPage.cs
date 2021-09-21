using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Errors
{
    public class AndroidCloseSlimTryAgainConnectionErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        public AndroidSlimCloseNavigation Navigation { get; set; }

        private AndroidCloseSlimTryAgainConnectionErrorPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidSlimCloseNavigation(driver);
        }

        public static AndroidCloseSlimTryAgainConnectionErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCloseSlimTryAgainConnectionErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Connection error");

        private AndroidLink TryAgainLink => AndroidLink.WithContentDescription(_driver, "Try again");

        public void TryAgain() => TryAgainLink.ScrollIntoView().Touch();
    }
}