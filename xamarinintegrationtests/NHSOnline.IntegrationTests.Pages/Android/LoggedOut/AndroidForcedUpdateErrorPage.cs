using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public class AndroidForcedUpdateErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidForcedUpdateErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Unable to verify app version");

        private AndroidLabel GoBackToHomeAndTryAgainLabel => AndroidLabel.WithText(_driver, "Go back to the home page and try again.");

        private AndroidLink BackToHomeLink => AndroidLink.WithText(_driver, "Back to home");

        public static AndroidForcedUpdateErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidForcedUpdateErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidForcedUpdateErrorPage AssertPageElements()
        {
            GoBackToHomeAndTryAgainLabel.AssertVisible();
            return this;
        }

        public void BackToHome() => BackToHomeLink.Touch();
    }
}