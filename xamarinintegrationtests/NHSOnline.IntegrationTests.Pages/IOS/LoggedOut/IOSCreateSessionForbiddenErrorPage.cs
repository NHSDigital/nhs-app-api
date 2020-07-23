using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionForbiddenErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionForbiddenErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Login failed");

        private IOSLabel CannotGetDetailsText => IOSLabel.WithText(_driver, "We cannot get your details from your GP surgery.");
        private IOSLabel GoBackText => IOSLabel.WithText(_driver, "Go back to the home screen and try logging in again.");

        private IOSLabel ErrorCodeText => IOSLabel.WhichMatches(_driver, "If you keep seeing this message, contact us. Quote the error code 3c[0-9a-z]{4} to help us resolve the problem more quickly.");

        private IOSLabel IfYouNeedText => IOSLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.");
        private IOSLink ContactUsLink => IOSLink.WithText(_driver, "Contact us");
        private IOSLink BackHomeLink => IOSLink.WithText(_driver, "Back to home");

        public static IOSCreateSessionForbiddenErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionForbiddenErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionForbiddenErrorPage AssertPageElements()
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