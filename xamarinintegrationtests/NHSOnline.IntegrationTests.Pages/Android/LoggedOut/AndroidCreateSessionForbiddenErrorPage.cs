using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionForbiddenErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionForbiddenErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Login failed");

        private AndroidLabel CannotGetDetailsText => AndroidLabel.WithText(_driver, "We cannot get your details from your GP surgery.");
        private AndroidLabel GoBackText => AndroidLabel.WithText(_driver, "Go back to the home screen and try logging in again.");

        private AndroidLabel ErrorCodeText => AndroidLabel.WhichMatches(_driver, "If you keep seeing this message, contact us. Quote the error code 3c[0-9a-z]{4} to help us resolve the problem more quickly.");

        private AndroidLabel IfYouNeedText => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.");
        private AndroidLabel ContactUsLink => AndroidLabel.WithText(_driver, "Contact us");
        private AndroidLabel BackHomeLink => AndroidLabel.WithText(_driver, "Back to home");

        public static AndroidCreateSessionForbiddenErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionForbiddenErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionForbiddenErrorPage AssertPageElements()
        {
            CannotGetDetailsText.AssertVisible();
            GoBackText.AssertVisible();
            ErrorCodeText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ContactUsLink.AssertVisible();
            BackHomeLink.AssertVisible();
            return this;
        }

        public void ContactUs()
        {
            ContactUsLink.Click();
        }

        public void BackHome()
        {
            BackHomeLink.Click();
        }
    }
}