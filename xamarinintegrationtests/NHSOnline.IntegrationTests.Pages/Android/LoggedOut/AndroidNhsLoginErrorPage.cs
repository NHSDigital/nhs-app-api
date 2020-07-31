using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidNhsLoginErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidNhsLoginErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Login failed");

        private AndroidLabel CannotGetDetailsText => AndroidLabel.WithText(_driver, "We cannot get your details from your GP surgery.");
        private AndroidLabel GoBackText => AndroidLabel.WithText(_driver, "Go back to the home screen and try logging in again.");
        private AndroidLabel ErrorCodeText => AndroidLabel.WhichMatches(_driver, "If you keep seeing this message, contact us. Quote the error code 3w[0-9a-z]{4} to help us resolve the problem quicker.");
        private AndroidLabel IfYouNeedText => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.");
        private AndroidLink ContactUsLink => AndroidLink.WithText(_driver, "Contact us");
        private AndroidLink BackHomeLink => AndroidLink.WithText(_driver, "Back home");

        public static AndroidNhsLoginErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNhsLoginErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidNhsLoginErrorPage AssertPageElements()
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
            ContactUsLink.Touch();
        }

        public void BackHome()
        {
            BackHomeLink.Touch();
        }
    }
}