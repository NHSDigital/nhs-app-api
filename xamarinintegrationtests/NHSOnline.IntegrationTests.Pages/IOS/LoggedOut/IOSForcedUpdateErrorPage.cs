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

        private IOSLabel Title=> IOSLabel.WithText(_driver, "Cannot log in");

        private IOSLabel CheckYouConnected => IOSLabel.WithText(_driver, "Check that you’re connected to the internet and go back to the login page to try again.");

        private IOSLabel IfYouNeedToBook => IOSLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");

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
            CheckYouConnected.AssertVisible();
            IfYouNeedToBook.AssertVisible();
            ForUrgentAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
            BackToLoginLink.AssertVisible();
            return this;
        }

        public void GoTo111() => GoTo111Link.Touch();

        public void BackToLogin() => BackToLoginLink.Touch();
    }
}