using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionBadRequestErrorPage
    {
        private readonly IIOSDriverWrapper _driver;
        private IOSCreateSessionBadRequestErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Cannot log in");
        private IOSLabel GoBackText => IOSLabel.WithText(_driver, "Go back and try logging in again.");
        private IOSLabel IfYouNeedText => IOSLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");
        private IOSLabel ForUrgentMedicalAdvice => IOSLabel.WithText(_driver, "For urgent medical advice, use NHS 111 online or call 111.");
        private IOSLink GoTo111Link => IOSLink.WithText(_driver, "Go to 111.nhs.uk");
        private IOSLink ContactUsLink => IOSLink.WhichMatches(_driver, "Contact us if you keep seeing this message, quoting error code 3 a ([0-9a-z] ){4}");
        private IOSLink BackToLoginLink => IOSLink.WithText(_driver, "Back to login");

       public static IOSCreateSessionBadRequestErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionBadRequestErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionBadRequestErrorPage AssertPageElements()
        {
            GoBackText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ForUrgentMedicalAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
            ContactUsLink.AssertVisible();
            BackToLoginLink.AssertVisible();
            return this;
        }

        public void ContactUs() => ContactUsLink.Touch();
        public void BackToLogin() => BackToLoginLink.Touch();
    }
}