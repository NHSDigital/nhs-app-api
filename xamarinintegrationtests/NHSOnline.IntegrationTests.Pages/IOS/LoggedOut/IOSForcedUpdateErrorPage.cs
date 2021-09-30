using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public class IOSForcedUpdateErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        public IOSSlimCloseNavigation Navigation { get; }

        private IOSForcedUpdateErrorPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSSlimCloseNavigation(driver);
        }

        private IOSLabel Title=> IOSLabel.WithText(_driver, "Unable to confirm NHS App version");

        private IOSLabel WeCannotConfirm => IOSLabel.WithText(_driver, "There is a problem connecting you. We cannot confirm which version of the NHS App you are using and if you may need to update it.");

        private IOSLabel CheckYourConnection => IOSLabel.WithText(_driver, "Check your connection and try logging in again.");

        private IOSLabel IfTheProblemContinues => IOSLabel.WithText(_driver, "If the problem continues and you need to book and appointment or get a prescription now, contact your GP surgery directly.");

        private IOSLabel ForUrgentAdvice => IOSLabel.WithText(_driver, "For urgent medical advice, use NHS 111 online or call 111.");

        private IOSLink GoTo111Link => IOSLink.WithText(_driver, "Go to 111.nhs.uk");

        private IOSLink BackToLoginLink => IOSLink.WithText(_driver, "Back to login");

        public static IOSForcedUpdateErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSForcedUpdateErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSForcedUpdateErrorPage AssertPageElements()
        {
            WeCannotConfirm.AssertVisible();
            CheckYourConnection.AssertVisible();
            IfTheProblemContinues.AssertVisible();
            ForUrgentAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
            BackToLoginLink.AssertVisible();
            return this;
        }

        public void GoTo111() => GoTo111Link.Touch();

        public void BackToLogin() => BackToLoginLink.Touch();
    }
}