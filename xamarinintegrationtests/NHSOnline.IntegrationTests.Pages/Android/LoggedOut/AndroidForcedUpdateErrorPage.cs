using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public class AndroidForcedUpdateErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        public AndroidSlimCloseNavigation Navigation { get; }

        private AndroidForcedUpdateErrorPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidSlimCloseNavigation(driver);
        }

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Unable to confirm NHS App version");

        private AndroidLabel WeCannotConfirm => AndroidLabel.WithText(_driver, "There is a problem connecting you. We cannot confirm which version of the NHS App you are using and if you may need to update it.");

        private AndroidLabel CheckYourConnection => AndroidLabel.WithText(_driver, "Check your connection and try logging in again.");

        private AndroidLabel IfTheProblemContinues => AndroidLabel.WithText(_driver, "If the problem continues and you need to book and appointment or get a prescription now, contact your GP surgery directly.");

        private AndroidLabel ForUrgentAdvice => AndroidLabel.WithText(_driver, "For urgent medical advice, use NHS 111 online or call 111.");

        private AndroidLink GoTo111Link => AndroidLink.WithContentDescription(_driver, "Go to 111.nhs.uk").ScrollIntoView();

        private AndroidLink BackToLoginLink => AndroidLink.WithContentDescription(_driver, "Back to login").ScrollIntoView();

        public static AndroidForcedUpdateErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidForcedUpdateErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidForcedUpdateErrorPage AssertPageElements()
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