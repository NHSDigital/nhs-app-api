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

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Cannot log in");

        private AndroidLabel CheckYouConnected => AndroidLabel.WithText(_driver, "Check that you’re connected to the internet and go back to the login page to try again.");

        private AndroidLabel IfYouNeedToBook => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");

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