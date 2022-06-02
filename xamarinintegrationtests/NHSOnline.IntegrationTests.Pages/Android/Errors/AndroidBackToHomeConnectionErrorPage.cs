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

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Cannot connect to the NHS App");

        private AndroidLink GoTo111Link => AndroidLink.WithContentDescription(_driver, "Go to 111.nhs.uk");

        private AndroidLink BackToHomeLink => AndroidLink.WithContentDescription(_driver, "Back to home");

        public void GoTo111() => GoTo111Link.ScrollIntoView().Touch();

        public void BackToHome() => BackToHomeLink.ScrollIntoView().Touch();
    }
}