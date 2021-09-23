using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Errors
{
    public class AndroidBackToHomeConnectionErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidBackToHomeConnectionErrorPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        public static AndroidBackToHomeConnectionErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidBackToHomeConnectionErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Connection error");

        private AndroidLink BackToHomeLink => AndroidLink.WithText(_driver, "Back to home");

        public void BackToHome() => BackToHomeLink.ScrollIntoView().Touch();
    }
}