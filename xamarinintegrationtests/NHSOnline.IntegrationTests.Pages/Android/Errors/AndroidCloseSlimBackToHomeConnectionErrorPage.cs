using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Errors
{
    public class AndroidCloseSlimBackToHomeConnectionErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        public AndroidSlimCloseNavigation Navigation { get; set; }

        private AndroidCloseSlimBackToHomeConnectionErrorPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidSlimCloseNavigation(driver);
            _driver = driver;
        }

        public static AndroidCloseSlimBackToHomeConnectionErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCloseSlimBackToHomeConnectionErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Connection error");

        private AndroidLink BackToHomeLink => AndroidLink.WithText(_driver, "Back to home");

        public void BackToHome() => BackToHomeLink.ScrollIntoView().Touch();
    }
}