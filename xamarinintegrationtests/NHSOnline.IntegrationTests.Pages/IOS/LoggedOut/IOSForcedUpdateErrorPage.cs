using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public class IOSForcedUpdateErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSForcedUpdateErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Unable to verify app version");

        private IOSLabel GoBackToHomeAndTryAgainLabel => IOSLabel.WithText(_driver, "Go back to the home page and try again.");

        private IOSLink BackToHomeLink => IOSLink.WithText(_driver, "Back to home");

        public static IOSForcedUpdateErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSForcedUpdateErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSForcedUpdateErrorPage AssertPageElements()
        {
            GoBackToHomeAndTryAgainLabel.AssertVisible();
            return this;
        }

        public void BackToHome() => BackToHomeLink.Touch();
    }
}