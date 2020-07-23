using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionBadResponseFromUpstreamSystemErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionBadResponseFromUpstreamSystemErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Login failed");

        private IOSLabel ThisCouldBeText => IOSLabel.WithText(_driver, "This can be one of two problems:");
        private IOSLabel CannotGetLoginDetailsText => IOSLabel.WithText(_driver, "we cannot get your NHS login details");
        private IOSLabel CannotConnectToGpSurgeryText => IOSLabel.WithText(_driver, "we cannot connect to your GP surgery");

        private IOSLabel GoBackText => IOSLabel.WithText(_driver, "Go back to the home screen and try logging in again.");

        private IOSLabel ErrorCodeText => IOSLabel.WhichMatches(_driver, "If you keep seeing this message, contact us. Quote the error code 3[0-9a-z]{5} to help us resolve the problem more quickly.");

        private IOSLabel IfYouNeedText => IOSLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.");
        private IOSLink ContactUsLink => IOSLink.WithText(_driver, "Contact us");
        private IOSLink BackHomeLink => IOSLink.WithText(_driver, "Back to home");

        public static IOSCreateSessionBadResponseFromUpstreamSystemErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionBadResponseFromUpstreamSystemErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionBadResponseFromUpstreamSystemErrorPage AssertPageElements()
        {
            ThisCouldBeText.AssertVisible();
            CannotGetLoginDetailsText.AssertVisible();
            CannotConnectToGpSurgeryText.AssertVisible();
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