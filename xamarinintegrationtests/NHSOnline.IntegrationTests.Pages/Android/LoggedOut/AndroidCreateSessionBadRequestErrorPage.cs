using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionBadRequestErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionBadRequestErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Login failed");

        private AndroidLabel GoBackText => AndroidLabel.WithText(_driver, "Go back to the home screen and try logging in again.");

        private AndroidLabel ErrorCodeText => AndroidLabel.WhichMatches(_driver, "If you keep seeing this message, contact us. Quote the error code 3a[0-9a-z]{4} to help us resolve the problem more quickly.");

        private AndroidLabel IfYouNeedText => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.");
        private AndroidLink ContactUsLink => AndroidLink.WithText(_driver, "Contact us");
        private AndroidLink BackHomeLink => AndroidLink.WithText(_driver, "Back to home");

        public static AndroidCreateSessionBadRequestErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionBadRequestErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionBadRequestErrorPage AssertPageElements()
        {
            GoBackText.AssertVisible();
            ErrorCodeText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ContactUsLink.AssertVisible();
            BackHomeLink.AssertVisible();
            return this;
        }

        public void ContactUs()
        {
            ContactUsLink.Touch();
        }

        public void BackHome()
        {
            BackHomeLink.Touch();
        }
    }
}